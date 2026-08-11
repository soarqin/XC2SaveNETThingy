# Save As Round-Trip Preservation Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Loading `bf2savefile.sav` and saving it without edits must preserve the original save payload byte-for-byte after accounting for the file's compression wrapper.

**Architecture:** First isolate serialization-only changes from WinForms load-time mutations using the existing `XC2SaveRoundTrip` tool. Add a regression test around the exact fixture and report the first differing offset. The serializer was lossless after preserving `PaddedString` slots; the remaining mutation was traced through the real Bf2 form load path and fixed with a loading guard around float/control event writes.

**Tech Stack:** .NET/C#, `XC2SaveSerialization`, WinForms Bf2 editor, console self-tests.

---

### Task 1: Establish the failing round-trip reproduction

**Files:**
- Inspect: `bf2savefile.sav`
- Inspect: `tools/XC2SaveRoundTrip/RawSaveInspector.cs`
- Modify: `tools/XC2SaveRoundTrip/RoundTripSelfTests.cs`

- [x] **Step 1: Run the existing inspector against the fixture**

Run:

```powershell
$roundTripOutput = Join-Path ([IO.Path]::GetTempPath()) "xc2-roundtrip-check"
New-Item -ItemType Directory -Force $roundTripOutput | Out-Null
dotnet run --project .\tools\XC2SaveRoundTrip\XC2SaveRoundTrip.csproj -- --input-dir . --output-dir $roundTripOutput
```

Record whether the generated output differs in raw payload length, compression state, or byte content.

- [x] **Step 2: Add a fixture-specific failing assertion**

Extend `RoundTripSelfTests` with one test that loads `bf2savefile.sav`, serializes the deserialized object without changing its compression payload, and asserts equality with the decompressed source payload. The assertion must include the first differing offset and both byte values when it fails.

- [x] **Step 3: Run the new test and confirm it fails for the observed reason**

Run:

```powershell
dotnet run --project .\tools\XC2SaveRoundTrip\XC2SaveRoundTrip.csproj -- --self-test
```

Expected: the new preservation test fails because the current serializer or form path changes bytes.

### Task 2: Locate the mutation boundary

**Files:**
- Inspect: `XC2SaveNETThingy/SaveObjects/XC2Save.cs`
- Inspect: `XC2Bf2SaveNETThingy/frmMain.cs`
- Modify only the diagnostic/test files needed to identify the first mutation.

- [x] **Step 1: Compare deserialize/serialize output independently of WinForms**

Use `XC2SaveSerialization.Deserialize` followed immediately by `XC2SaveSerialization.Serialize(save, false)` and compare the raw payload. If this is equal, treat WinForms initialization as the active boundary; if it differs, trace `ToRawData()` and version-specific serialization fields.

- [x] **Step 2: Trace Bf2 initialization event writes**

Inspect every `ValueChanged`, `SelectedIndexChanged`, `SelectedValueChanged`, `CheckedChanged`, and data-binding callback reachable during `LoadSaveData`. Identify the first handler that writes to `SAVEDATA` while controls are being populated.

- [x] **Step 3: Record the single root-cause hypothesis**

The first serializer-only mutation was caused by `PaddedString.ToRawData()` discarding non-zero padding/trailing bytes in Blade resource-name slots; that was fixed and covered. The actual WinForms-only mutation was 8 bytes at raw offsets `0x000028`, `0x0060AA`, `0x109800`, `0x109804`, `0x109830`, `0x109834`, `0x10AD50`, and `0x10AE84`, caused by initialization events writing rounded float values and one Blade favorite category.

### Task 3: Add the minimal regression fix

**Files:**
- Modify: `XC2Bf2SaveNETThingy/frmMain.cs` if initialization events mutate `SAVEDATA`.
- Modify: `XC2SaveNETThingy/SaveObjects/XC2Save.cs` only if serialization itself loses source bytes.
- Modify: `tools/XC2SaveRoundTrip/RoundTripSelfTests.cs` to keep the fixture regression test.

- [x] **Step 1: Implement one minimal root-cause fix**

`PaddedString` retains the original fixed-size byte slot while its parsed string and length remain unchanged. Bf2 `LoadSaveData` sets `IsLoadingSaveData`, and float/favorite-category handlers ignore initialization events until loading completes; user edits after loading still write normally. Do not bundle unrelated UI refactoring.

- [x] **Step 2: Run the fixture regression test and confirm it passes**

Run:

```powershell
dotnet run --project .\tools\XC2SaveRoundTrip\XC2SaveRoundTrip.csproj -- --self-test
```

Expected: the fixture preservation test and all existing round-trip tests pass.

The regression also invokes `frmMain_Load` and private `LoadSaveData` through the Bf2 test harness, then serializes the form's actual `SAVEDATA` object.

### Task 4: Verify the whole repository state

**Files:**
- Inspect all modified files and the final diff.

- [x] **Step 1: Build the solution**

```powershell
dotnet build .\XC2SaveNETThingy.sln --no-restore
```

Expected: exit code 0 with no warnings or errors.

- [x] **Step 2: Run all relevant self-tests**

```powershell
dotnet run --project .\tools\XC2SaveRoundTrip\XC2SaveRoundTrip.csproj -- --self-test
dotnet run --project .\tools\XC2BdatImporter\XC2BdatImporter.csproj -- --self-test
git diff --check
```

- [x] **Step 3: Review the final diff**

Confirm the fixture and `romdata/` remain untouched as user-provided files, and the change is limited to the confirmed Save As preservation defect plus its regression test.
