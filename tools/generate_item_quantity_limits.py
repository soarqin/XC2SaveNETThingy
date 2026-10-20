"""Rebuild quantity rules from the verified XC2 3.0 ELF and common.bdat.

See docs/xc2-item-quantity-limits.md for the reverse-engineered call paths.
No save-file quantities or item-name heuristics are used.
"""

import argparse
import collections
import csv
import hashlib
import io
import json
from pathlib import Path
import struct
import subprocess
import tempfile


ROOT = Path(__file__).resolve().parents[1]
# ponytail: this decoder is pinned to the verified 3.0 ELF; re-audit offsets for another build.
ELF_SHA256 = "7257530fd99830977b04a5b8b3f699c2e6ef283bb550c5d67b80b52ebd0f7537"
COLUMNS = ("ID", "Type", "MaxQty", "BonusType", "BonusValue", "BonusMax")


def read_elf(data, address, size):
    assert data[:6] == b"\x7fELF\x02\x01", "Expected little-endian ELF64"
    offset = struct.unpack_from("<Q", data, 32)[0]
    entry_size, count = struct.unpack_from("<HH", data, 54)
    for index in range(count):
        kind, _, file_offset, virtual, _, file_size, _, _ = struct.unpack_from(
            "<IIQQQQQQ", data, offset + index * entry_size
        )
        if kind == 1 and virtual <= address and address + size <= virtual + file_size:
            start = file_offset + address - virtual
            return data[start:start + size]
    raise ValueError(f"ELF address is not backed by file data: {address:#x}")


def inventory_defaults(elf):
    # Read the actual W1 (box), W6 (stack cap), W7 (stackable) arguments
    # passed by sub_2F3F98, including MOV aliases for ORR immediates.
    registers = [None] * 31 + [0]
    boxes = {}
    for index, (word,) in enumerate(struct.iter_unpack("<I", read_elf(elf, 0x2F3F98, 0x35C))):
        destination = word & 31
        opcode = word & 0x7F800000
        shift = ((word >> 21) & 3) * 16
        value = (word >> 5) & 0xFFFF
        if opcode == 0x52800000:  # MOVZ
            registers[destination] = value << shift
        elif opcode == 0x72800000:  # MOVK
            previous = registers[destination]
            registers[destination] = None if previous is None else (previous & ~(0xFFFF << shift)) | (value << shift)
        elif word & 0x7FE0FFE0 == 0x2A0003E0:  # MOV Wd, Wm / Xd, Xm
            registers[destination] = registers[(word >> 16) & 31]
        elif word & 0x7F8003E0 == 0x320003E0:  # ORR Wd/Xd, WZR/XZR, #bitmask
            immediate = (word >> 10) & 63
            length = (((word >> 22) & 1) << 6 | (~immediate & 63)).bit_length() - 1
            bits = 1 << length
            levels = bits - 1
            rotation = (word >> 16) & levels
            pattern = (1 << ((immediate & levels) + 1)) - 1
            pattern = ((pattern >> rotation) | (pattern << (bits - rotation))) & ((1 << bits) - 1)
            registers[destination] = sum(pattern << shift for shift in range(0, 64 if word >> 31 else 32, bits))
        if word & 0xFC000000 in (0x94000000, 0x14000000):  # BL / tail B
            displacement = word & 0x3FFFFFF
            if displacement & 0x2000000:
                displacement -= 0x4000000
            target = 0x2F3F98 + index * 4 + displacement * 4
            if 0x2F42F4 <= target <= 0x2F7DF4:
                box, cap, stackable = (registers[i] for i in (1, 6, 7))
                assert box is not None and cap is not None and stackable in (0, 1), f"Unresolved box arguments at {target:#x}"
                assert box not in boxes, f"Duplicate inventory box {box}"
                boxes[box] = (cap, stackable)
            registers[:19] = [None] * 19
        registers[31] = 0
    assert set(boxes) == set(range(1, 19)), "Inventory initializer no longer matches the verified build"
    type_to_box = struct.unpack("<20I", read_elf(elf, 0x9BBFA0, 80))
    return {kind: boxes[box] for kind, box in enumerate(type_to_box, 1) if box}


def generate(elf, directory):
    defaults = inventory_defaults(elf)
    mappings = json.loads((ROOT / "tools/XC2BdatImporter/mappings.json").read_text(encoding="utf-8"))
    tables = {}

    def table(name):
        if name not in tables:
            tables[name] = json.loads((directory / "common" / (name + ".json")).read_text(encoding="utf-8"))["rows"]
        return tables[name]

    # The raw EventBox has no name mapping in the editor, but is still an
    # inventory category (type 15 in sub_232C64) and must not be omitted.
    mappings.append({"sourceTable": "ITM_EventList", "itemTypeId": 15,
                     "idOffset": table("ITM_EventList")[0]["$id"] - 1})
    bonuses = {row["$id"]: row for row in table("FLD_OwnerBonus")}
    bonus_caps = {row["$id"]: row["Max"] for row in table("FLD_OwnerBonusParam")}
    # sub_2FABA4 reads signed halfwords at +6 (pouch) and +26 (booster)
    # in the owner-bonus array. Decode those indices from its LDRSH instructions.
    bonus_types = {}
    for kind, address in ((10, 0x2FAC48), (12, 0x2FAC68)):
        word = struct.unpack("<I", read_elf(elf, address, 4))[0]
        assert word & 0xFFC00000 == 0x79C00000, "Expected LDRSH Wt, [Xn, #imm]"
        bonus_types[kind] = (word >> 10) & 0xFFF
    booster_fallback = struct.unpack("<I", read_elf(elf, 0x2FAC70, 4))[0]
    assert booster_fallback & 0xFFE0001F == 0x52800008, "Expected MOV W8, #imm"
    booster_fallback = (booster_fallback >> 5) & 0xFFFF

    result = {}
    counts = collections.Counter()
    for mapping in mappings:
        if "itemTypeId" not in mapping:
            continue
        kind = mapping["itemTypeId"]
        default, stackable = defaults[kind]
        for row in table(mapping["sourceTable"]):
            item_id = row["$id"] - mapping.get("idOffset", 0)
            assert 0 < item_id <= 8191
            cap = row["ValueMax"] if kind in (6, 10, 11, 12) else default
            if kind == 12 and not cap:
                cap = booster_fallback
            cap = max(1, cap) if stackable else 1
            bonus_type, bonus_value, bonus_cap = 0, 0, 0
            if kind in bonus_types:
                bonus_type = bonus_types[kind]
                bonus_cap = bonus_caps[bonus_type]
            elif kind == 6 and row["Category"] == 28 and row["Type"]:
                bonus = bonuses[row["Type"]]
                if bonus["Type"] in bonus_types.values():
                    bonus_type = bonus["Type"]
                    bonus_value = bonus["Value"]
                    bonus_cap = bonus_caps[bonus_type]
            key = (item_id, kind)
            assert key not in result, f"Duplicate item {key}"
            assert cap + bonus_cap <= 1023
            result[key] = (item_id, kind, cap, bonus_type, bonus_value, bonus_cap)
            counts[kind] += 1
    assert len(result) == 4013 and len(counts) == 18, "The verified 3.0 item coverage changed"
    print("Covered item records by type:", dict(sorted(counts.items())))
    output = io.StringIO(newline="")
    writer = csv.writer(output, delimiter="\t", lineterminator="\r\n")
    writer.writerow(COLUMNS)
    writer.writerows(result[key] for key in sorted(result, key=lambda key: (key[1], key[0])))
    return output.getvalue().encode("utf-16")


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    mode = parser.add_mutually_exclusive_group(required=True)
    mode.add_argument("--check", action="store_true", help="compare all generated records with the project CSV")
    mode.add_argument("--write", action="store_true", help="replace only ItemQuantityLimits.csv")
    parser.add_argument("--common", type=Path, default=ROOT / "romdata/3.0/bdat/common.bdat")
    args = parser.parse_args()
    elf = (ROOT / "romdata/3.0/main.elf").read_bytes()
    if hashlib.sha256(elf).hexdigest() != ELF_SHA256:
        raise SystemExit("Unverified ELF build; recheck the code addresses first.")
    with tempfile.TemporaryDirectory(prefix="xc2-item-limits-") as temporary:
        subprocess.run([
            str(ROOT / "romdata/bdat-toolset-win64.exe"), "extract", "--file-type", "json",
            "--out-dir", temporary, str(args.common.resolve()),
        ], check=True)
        generated = generate(elf, Path(temporary))
    target = ROOT / "XC2SaveNETThingy/data/ItemQuantityLimits.csv"
    if args.write:
        target.write_bytes(generated)
    elif target.read_bytes() != generated:
        raise SystemExit("Item quantity rules differ from the ELF/BDAT reconstruction; run with --write after review.")
    print("ITEM_QUANTITY_RULES_OK", "written" if args.write else "verified", "ELF", ELF_SHA256,
          "BDAT", hashlib.sha256(args.common.read_bytes()).hexdigest())


if __name__ == "__main__":
    main()
