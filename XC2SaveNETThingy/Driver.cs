using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XC2SaveNETThingy
{
    public class Driver : IXC2SaveObject
    {
        public static int SIZE = 0x5A0;

        public static readonly Dictionary<UInt16, string> IDNAMES = new Dictionary<UInt16, string>()
        {
            { 0, "(none)" },
            { 1, "Rex" },
            { 2, "Nia" },
            { 3, "Zeke" },
            { 4, "Tora" },
            { 5, "Vandham" },
            { 6, "Mòrag" },
            { 7, "Jin (Ch 1 Guest)" },
            { 8, "Malos (Ch 1 Guest)" },
            { 9, "Jin (Ch 8 Guest)" },
            { 10, "Unknown1" },
            { 11, "Unknown2" },
            { 12, "Unknown3" },
            { 13, "Unknown4" },
            { 14, "Unknown5" },
            { 15, "Unknown6" },
            { 16, "Unknown7" }
        };

        public static readonly Dictionary<int, string> STATUS = new Dictionary<int, string>()
        {
            { 0, "Not Active" },
            { 1, "In Party" },
            { 2, "In Mercenary Team" }
        };

        public class GfDataDriverSkill : IXC2SaveObject
        {
            public const int SIZE = 0xA;
            public UInt16[] Columns = new UInt16[3];
            public Int16 RowNum;
            public UInt16 Level;

            public GfDataDriverSkill(Byte[] data)
            {
                Columns = new UInt16[3];
                for (int i = 0; i < Columns.Length; i++)
                {
                    Columns[i] = BitConverter.ToUInt16(data.GetByteSubArray(i * 2, 2), 0);
                }
                RowNum = BitConverter.ToInt16(data.GetByteSubArray(0x6, 2), 0);
                Level = BitConverter.ToUInt16(data.GetByteSubArray(0x8, 2), 0);
            }

            public Byte[] ToRawData()
            {
                List<Byte> result = new List<byte>();

                for (int i = 0; i < Columns.Length; i++)
                    result.AddRange(BitConverter.GetBytes(Columns[i]));

                result.AddRange(BitConverter.GetBytes(RowNum));
                result.AddRange(BitConverter.GetBytes(Level));

                return result.ToArray();
            }
        }

        public class Weapon : IXC2SaveObject
        {
            public static readonly Dictionary<int, string> NAMES = new Dictionary<int, string>()
            {
                { 0, "(none)"},
                { 1, "Aegis Sword"},
                { 2, "Catalyst Scimitar"},
                { 3, "Twin Rings"},
                { 4, "Drill Shield"},
                { 5, "Mech Arms"},
                { 6, "Variable Saber"},
                { 7, "Whipswords"},
                { 8, "Big Bang Edge"},
                { 9, "Dual Scythes"},
                { 10, "Greataxe"},
                { 11, "Megalance"},
                { 12, "Ether Cannon"},
                { 13, "Shield Hammer"},
                { 14, "Chroma Katana"},
                { 15, "Bitball"},
                { 16, "Knuckle Claws"},
                { 17, "Broadsword"},
                { 18, "JIN_WEAPON"},
                { 19, "MALOS_WEAPON"},
                { 20, "Calamity Scythe"},
                { 21, "Cobra Bardiche"},
                { 22, "22"},
                { 23, "Brilliant Twinblades"},
                { 24, "24"},
                { 25, "25"},
                { 26, "26"}
            };

            public const int SIZE = 0x14;

            public UInt32[] ArtIds;
            public int WeaponPoints;
            public int TotalWeaponPoints;

            public Weapon(Byte[] data)
            {
                ArtIds = new UInt32[3];
                for (int i = 0; i < ArtIds.Length; i++)
                {
                    ArtIds[i] = BitConverter.ToUInt32(data.GetByteSubArray(i * 4, 4), 0);
                }
                WeaponPoints = BitConverter.ToInt32(data.GetByteSubArray(0xC, 4), 0);
                TotalWeaponPoints = BitConverter.ToInt32(data.GetByteSubArray(0x10, 4), 0);
            }

            public Byte[] ToRawData()
            {
                List<Byte> result = new List<Byte>();

                for (int i = 0; i < ArtIds.Length; i++)
                    result.AddRange(BitConverter.GetBytes(ArtIds[i]));

                result.AddRange(BitConverter.GetBytes(WeaponPoints));
                result.AddRange(BitConverter.GetBytes(TotalWeaponPoints));

                return result.ToArray();
            }
        }

        public class SDataPouch : IXC2SaveObject
        {
            public const int SIZE = 0x8;
            float Time;
            UInt16 ItemId;
            UInt16 IsEnabled;

            public SDataPouch(Byte[] data)
            {
                Time = BitConverter.ToSingle(data.GetByteSubArray(0, 4), 0);
                ItemId = BitConverter.ToUInt16(data.GetByteSubArray(4, 2), 0);
                IsEnabled = BitConverter.ToUInt16(data.GetByteSubArray(6, 2), 0);
            }

            public Byte[] ToRawData()
            {
                List<Byte> result = new List<Byte>();

                result.AddRange(BitConverter.GetBytes(Time));
                result.AddRange(BitConverter.GetBytes(ItemId));
                result.AddRange(BitConverter.GetBytes(IsEnabled));

                return result.ToArray();
            }
        }

        private static Dictionary<string, int> LOC = new Dictionary<string, int>()
        {
            { "BraveryPoints", 0x0 },
            { "BraveryLevel", 0x4 },
            { "TruthPoints", 0x8 },
            { "TruthLevel", 0xC },
            { "CompassionPoints", 0x10 },
            { "CompassionLevel", 0x14 },
            { "JusticePoints", 0x18 },
            { "JusticeLevel", 0x1C },
            { "IsActive", 0x20 },
            { "DriverId", 0x22 },
            { "SetBlade", 0x24 },
            { "EquippedBlades", 0x26 },
            { "gap_2C", 0x2C },
            { "SkillsRound1", 0x30 },
            { "SkillsRound2", 0x62 },
            { "Level", 0x94 },
            { "HpMax", 0x96 },
            { "Strength", 0x98 },
            { "PowEther", 0x9A },
            { "Dex", 0x9C },
            { "Agility", 0x9E },
            { "Luck", 0xA0 },
            { "PArmor", 0xA2 },
            { "EArmor", 0xA4 },
            { "CritRate", 0xA6 },
            { "GuardRate", 0xA7 },
            { "field_A8", 0xA8 },
            { "gap_AC", 0xAC },
            { "field_AF", 0xAF },
            { "Exp", 0xB0 },
            { "BattleExp", 0xB4 },
            { "SkillPoints", 0xB8 },
            { "TotalSkillPoints", 0xBC },
            { "Weapons", 0xC0 },
            { "gap_2DC", 0x2DC },
            { "AccessoryId2", 0x2E8 },
            { "gap_2EA", 0x2EA },
            { "AccessoryHandle2", 0x2EC },
            { "DriverArtLevels", 0x2F0 },
            { "gap_4FD", 0x4FD },
            { "AccessoryHandle0", 0x574 },
            { "AccessoryId0", 0x578 },
            { "gap_57A", 0x57A },
            { "AccessoryHandle1", 0x57C },
            { "AccessoryId1", 0x580 },
            { "gap_582", 0x582 },
            { "PouchInfo", 0x584 },
            { "gap_field_59A", 0x59A }
        };

        public UInt32 BraveryPoints;
        public UInt32 BraveryLevel;
        public UInt32 TruthPoints;
        public UInt32 TruthLevel;
        public UInt32 CompassionPoints;
        public UInt32 CompassionLevel;
        public UInt32 JusticePoints;
        public UInt32 JusticeLevel;
        public UInt16 IsActive;
        public UInt16 DriverId;
        public Int16 SetBlade;
        public UInt16[] EquippedBlades = new UInt16[3];
        public UInt32 gap_2C;
        public GfDataDriverSkill[] SkillsRound1 = new GfDataDriverSkill[5];
        public GfDataDriverSkill[] SkillsRound2 = new GfDataDriverSkill[5];
        public UInt16 Level;
        public UInt16 HpMax;
        public UInt16 Strength;
        public UInt16 PowEther;
        public UInt16 Dex;
        public UInt16 Agility;
        public UInt16 Luck;
        public UInt16 PArmor;
        public UInt16 EArmor;
        public Byte CritRate;
        public Byte GuardRate;
        public UInt32 field_A8;
        public Byte[] gap_AC = new Byte[3];
        public Byte field_AF;
        public UInt32 Exp;
        public UInt32 BattleExp;
        public UInt32 SkillPoints;
        public UInt32 TotalSkillPoints;
        public Weapon[] Weapons = new Weapon[27];
        public Byte[] gap_2DC = new Byte[12];
        public UInt16 AccessoryId2;
        public Byte[] gap_2EA = new Byte[2];
        public ItemHandle AccessoryHandle2;
        public Byte[] DriverArtLevels = new Byte[525];
        public Byte[] gap_4FD = new Byte[119];
        public ItemHandle AccessoryHandle0;
        public UInt16 AccessoryId0;
        public UInt16 gap_57A;
        public ItemHandle AccessoryHandle1;
        public UInt16 AccessoryId1;
        public UInt16 gap_582;
        public SDataPouch[] PouchInfo = new SDataPouch[3];
        public Byte[] gap_field_59A = new Byte[3];

        public Driver(Byte[] data)
        {
            BraveryPoints = BitConverter.ToUInt32(data.GetByteSubArray(LOC["BraveryPoints"], 4), 0);
            BraveryLevel = BitConverter.ToUInt32(data.GetByteSubArray(LOC["BraveryLevel"], 4), 0);
            TruthPoints = BitConverter.ToUInt32(data.GetByteSubArray(LOC["TruthPoints"], 4), 0);
            TruthLevel = BitConverter.ToUInt32(data.GetByteSubArray(LOC["TruthLevel"], 4), 0);
            CompassionPoints = BitConverter.ToUInt32(data.GetByteSubArray(LOC["CompassionPoints"], 4), 0);
            CompassionLevel = BitConverter.ToUInt32( data.GetByteSubArray(LOC["CompassionLevel"], 4), 0);
            JusticePoints = BitConverter.ToUInt32(data.GetByteSubArray(LOC["JusticePoints"], 4), 0);
            JusticeLevel = BitConverter.ToUInt32(data.GetByteSubArray(LOC["JusticeLevel"], 4), 0);
            IsActive = BitConverter.ToUInt16(data.GetByteSubArray(LOC["IsActive"], 2), 0);
            DriverId = BitConverter.ToUInt16(data.GetByteSubArray(LOC["DriverId"], 2), 0);
            SetBlade = BitConverter.ToInt16(data.GetByteSubArray(LOC["SetBlade"], 2), 0);

            EquippedBlades = new UInt16[3];
            for (int i = 0; i < EquippedBlades.Length; i++)
                EquippedBlades[i] = BitConverter.ToUInt16(data.GetByteSubArray(LOC["EquippedBlades"] + (i * 2), 2), 0);

            gap_2C = BitConverter.ToUInt32(data.GetByteSubArray(LOC["gap_2C"], 4), 0);

            SkillsRound1 = new GfDataDriverSkill[5];
            for (int i = 0; i < SkillsRound1.Length; i++)
                SkillsRound1[i] = new GfDataDriverSkill(data.GetByteSubArray(LOC["SkillsRound1"] + (i * GfDataDriverSkill.SIZE), GfDataDriverSkill.SIZE));

            SkillsRound2 = new GfDataDriverSkill[5];
            for (int i = 0; i < SkillsRound2.Length; i++)
                SkillsRound2[i] = new GfDataDriverSkill(data.GetByteSubArray(LOC["SkillsRound2"] + (i * GfDataDriverSkill.SIZE), GfDataDriverSkill.SIZE));

            Level = BitConverter.ToUInt16(data.GetByteSubArray(LOC["Level"], 2), 0);
            HpMax = BitConverter.ToUInt16(data.GetByteSubArray(LOC["HpMax"], 2), 0);
            Strength = BitConverter.ToUInt16(data.GetByteSubArray(LOC["Strength"], 2), 0);
            PowEther = BitConverter.ToUInt16(data.GetByteSubArray(LOC["PowEther"], 2), 0);
            Dex = BitConverter.ToUInt16(data.GetByteSubArray(LOC["Dex"], 2), 0);
            Agility = BitConverter.ToUInt16(data.GetByteSubArray(LOC["Agility"], 2), 0);
            Luck = BitConverter.ToUInt16(data.GetByteSubArray(LOC["Luck"], 2), 0);
            PArmor = BitConverter.ToUInt16(data.GetByteSubArray(LOC["PArmor"], 2), 0);
            EArmor = BitConverter.ToUInt16(data.GetByteSubArray(LOC["EArmor"], 2), 0);
            CritRate = data[LOC["CritRate"]];
            GuardRate = data[LOC["GuardRate"]];
            field_A8 = BitConverter.ToUInt32(data.GetByteSubArray(LOC["field_A8"], 4), 0);

            gap_AC = new Byte[3];
            for (int i = 0; i < gap_AC.Length; i++)
                gap_AC[i] = data[LOC["gap_AC"] + i];

            field_AF = data[LOC["field_AF"]];
            Exp = BitConverter.ToUInt32(data.GetByteSubArray(LOC["Exp"], 4), 0);
            BattleExp = BitConverter.ToUInt32(data.GetByteSubArray(LOC["BattleExp"], 4), 0);
            SkillPoints = BitConverter.ToUInt32(data.GetByteSubArray(LOC["SkillPoints"], 4), 0);
            TotalSkillPoints = BitConverter.ToUInt32(data.GetByteSubArray(LOC["TotalSkillPoints"], 4), 0);

            Weapons = new Weapon[27];
            for (int i = 0; i < Weapons.Length; i++)
                Weapons[i] = new Weapon(data.GetByteSubArray(LOC["Weapons"] + (i * Weapon.SIZE), Weapon.SIZE));

            gap_2DC = new Byte[12];
            for (int i = 0; i < gap_2DC.Length; i++)
                gap_2DC[i] = data[LOC["gap_2DC"] + i];

            AccessoryId2 = BitConverter.ToUInt16(data.GetByteSubArray(LOC["AccessoryId2"], 2), 0);

            gap_2EA = new Byte[2];
            for (int i = 0; i < gap_2EA.Length; i++)
                gap_2EA[i] = data[LOC["gap_2EA"] + i];

            AccessoryHandle2 = new ItemHandle32(data.GetByteSubArray(LOC["AccessoryHandle2"], ItemHandle32.SIZE));

            DriverArtLevels = new Byte[525];
            for (int i = 0; i < DriverArtLevels.Length; i++)
                DriverArtLevels[i] = data[LOC["DriverArtLevels"] + i];

            gap_4FD = new Byte[119];
            for (int i = 0; i < gap_4FD.Length; i++)
                gap_4FD[i] = data[LOC["gap_4FD"] + i];

            AccessoryHandle0 = new ItemHandle32(data.GetByteSubArray(LOC["AccessoryHandle0"], ItemHandle32.SIZE));
            AccessoryId0 = BitConverter.ToUInt16(data.GetByteSubArray(LOC["AccessoryId0"], 2), 0);
            gap_57A = BitConverter.ToUInt16(data.GetByteSubArray(LOC["gap_57A"], 2), 0);
            AccessoryHandle1 = new ItemHandle32(data.GetByteSubArray(LOC["AccessoryHandle1"], ItemHandle32.SIZE));
            AccessoryId1 = BitConverter.ToUInt16(data.GetByteSubArray(LOC["AccessoryId1"], 2), 0);
            gap_582 = BitConverter.ToUInt16(data.GetByteSubArray(LOC["gap_582"], 2), 0);

            PouchInfo = new SDataPouch[3];
            for (int i = 0; i < PouchInfo.Length; i++)
                PouchInfo[i] = new SDataPouch(data.GetByteSubArray(LOC["PouchInfo"] + (i * SDataPouch.SIZE), SDataPouch.SIZE));

            gap_field_59A = new Byte[4];
            for (int i = 0; i < gap_field_59A.Length; i++)
                gap_field_59A[i] = data[LOC["gap_field_59A"] + i];
        }

        public Byte[] ToRawData()
        {
            List<Byte> result = new List<Byte>();

            result.AddRange(BitConverter.GetBytes(BraveryPoints));
            result.AddRange(BitConverter.GetBytes(BraveryLevel));
            result.AddRange(BitConverter.GetBytes(TruthPoints));
            result.AddRange(BitConverter.GetBytes(TruthLevel));
            result.AddRange(BitConverter.GetBytes(CompassionPoints));
            result.AddRange(BitConverter.GetBytes(CompassionLevel));
            result.AddRange(BitConverter.GetBytes(JusticePoints));
            result.AddRange(BitConverter.GetBytes(JusticeLevel));
            result.AddRange(BitConverter.GetBytes(IsActive));
            result.AddRange(BitConverter.GetBytes(DriverId));
            result.AddRange(BitConverter.GetBytes(SetBlade));

            for (int i = 0; i < EquippedBlades.Length; i++)
                result.AddRange(BitConverter.GetBytes(EquippedBlades[i]));

            result.AddRange(BitConverter.GetBytes(gap_2C));

            for (int i = 0; i < SkillsRound1.Length; i++)
                result.AddRange(SkillsRound1[i].ToRawData());

            for (int i = 0; i < SkillsRound2.Length; i++)
                result.AddRange(SkillsRound2[i].ToRawData());


            result.AddRange(BitConverter.GetBytes(Level));
            result.AddRange(BitConverter.GetBytes(HpMax));
            result.AddRange(BitConverter.GetBytes(Strength));
            result.AddRange(BitConverter.GetBytes(PowEther));
            result.AddRange(BitConverter.GetBytes(Dex));
            result.AddRange(BitConverter.GetBytes(Agility));
            result.AddRange(BitConverter.GetBytes(Luck));
            result.AddRange(BitConverter.GetBytes(PArmor));
            result.AddRange(BitConverter.GetBytes(EArmor));
            result.Add(CritRate);
            result.Add(GuardRate);
            result.AddRange(BitConverter.GetBytes(field_A8));

            for (int i = 0; i < gap_AC.Length; i++)
                result.Add(gap_AC[i]);

            result.Add(field_AF);
            result.AddRange(BitConverter.GetBytes(Exp));
            result.AddRange(BitConverter.GetBytes(BattleExp));
            result.AddRange(BitConverter.GetBytes(SkillPoints));
            result.AddRange(BitConverter.GetBytes(TotalSkillPoints));

            for (int i = 0; i < Weapons.Length; i++)
                result.AddRange(Weapons[i].ToRawData());

            for (int i = 0; i < gap_2DC.Length; i++)
                result.Add(gap_2DC[i]);

            result.AddRange(BitConverter.GetBytes(AccessoryId2));

            for (int i = 0; i < gap_2EA.Length; i++)
                result.Add(gap_2EA[i]);

            result.AddRange(AccessoryHandle2.ToRawData());

            for (int i = 0; i < DriverArtLevels.Length; i++)
                result.Add(DriverArtLevels[i]);

            for (int i = 0; i < gap_4FD.Length; i++)
                result.Add(gap_4FD[i]);

            result.AddRange(AccessoryHandle0.ToRawData());
            result.AddRange(BitConverter.GetBytes(AccessoryId0));
            result.AddRange(BitConverter.GetBytes(gap_57A));
            result.AddRange(AccessoryHandle1.ToRawData());
            result.AddRange(BitConverter.GetBytes(AccessoryId1));
            result.AddRange(BitConverter.GetBytes(gap_582));

            for (int i = 0; i < PouchInfo.Length; i++)
                result.AddRange(PouchInfo[i].ToRawData());

            for (int i = 0; i < gap_field_59A.Length; i++)
                result.Add(gap_field_59A[i]);

            return result.ToArray();
        }

        public override string ToString()
        {
            if (IDNAMES.ContainsKey(DriverId))
                return IDNAMES[DriverId];
            else
                return "INVALID ID";
        }
    }
}
