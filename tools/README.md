# Offline XC2 data and save verification tools

## BDAT importer

`XC2BdatImporter` builds a candidate `XC2Data.db` and mapped UTF-16 CSV files from the game BDAT resources. It joins gameplay records from `common.bdat` with the English `gb` language tables by the unsigned-short `Name` index; it does not match language rows by file order.

The importer accepts either a language directory or a version directory. When a version directory contains a `gb` child directory, that child is selected automatically.

```powershell
dotnet run --project .\tools\XC2BdatImporter -- `
  --base-bdat .\romdata\3.0\bdat `
  --base-gb .\romdata\3.0U\bdat `
  --torna-bdat .\romdata\3.0\bdat_torna `
  --torna-gb .\romdata\3.0U\bdat_torna `
  --toolset .\romdata\bdat-toolset-win64.exe `
  --output $env:TEMP\xc2-candidate `
  --report .\reports\xc2-3.0-data-diff.md `
  --verify-only
```

Only an explicit `--replace` updates project CSV files and `XC2SaveNETThingy/XC2Data.db`. The command creates a timestamped backup beside the candidate database before replacement. The Torna directories are validated as inputs, but Torna event, quest, camp-event, battle-text, and dialogue tables are not imported because the current editors do not read them.

Run `--self-test` for parser, mapping, database, and verification checks. The verifier requires SQLite integrity, primary-key, CSV/database, and foreign-key-baseline checks; the existing 1,811 foreign-key violations must not increase.

## Save round-trip verifier

`XC2SaveRoundTrip` reads raw or compressed saves from an external directory, deserializes and serializes each one, then compares the decompressed length, header, Drivers, Blades, ItemBox, ContentVersions, and SaveVersion regions. It writes a diagnostic round-trip file for each sample.

```powershell
dotnet run --project .\tools\XC2SaveRoundTrip -- `
  --input-dir C:\path\to\external-saves `
  --output-dir $env:TEMP\xc2-roundtrip `
  --compress
```

Without real samples the verifier can still run `--self-test` and synthetic structural checks, but that is not evidence of compatibility with an actual user save.
