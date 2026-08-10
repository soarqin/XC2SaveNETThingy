# Reserved Item Marking Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Add durable `Reserved` metadata for confirmed non-normal items and show a strong warning in both editors without changing save formats.

**Architecture:** Keep item identity and localized names in `Items`; add an independent `ItemStatus` table keyed by `ItemID`, with `Status`, bilingual reasons, and evidence. Extend shared `XC2Data` item queries with status and reuse the shared grid styling helper in the base and Torna editors. Generate a candidate report from multi-language evidence and validate it through the existing importer/database checks.

**Tech Stack:** C# WinForms, .NET 10, SQLite, UTF-16 tab-separated CSV, existing BDAT toolset and importer self-tests.

---

### Task 1: Add the Reserved schema and seed data

**Files:**
- Create: `XC2SaveNETThingy/db/Items_ItemStatus.sql`
- Create: `XC2SaveNETThingy/data/ItemStatus.csv`
- Modify: `XC2SaveNETThingy/XC2Data.db`
- Create: `reports/xc2-reserved-items.md`
- Create: `reports/xc2-reserved-items.json`

- [x] **Step 1: Write the failing schema/data check**

The initial check confirmed that the existing `Items` schema had no status column, which led to the independent-table design. The implemented self-test now exercises creation and persistence of `ItemStatus` instead of changing the generated item columns.

- [x] **Step 2: Add the independent `ItemStatus` table and confirmed rows**

Create `ItemStatus.csv` and `db/Items_ItemStatus.sql`. Add only the reviewed candidate IDs listed in `reports/xc2-reserved-items.json`, with `Status=Reserved` and the evidence fields. Do not change the `Items` schema or item CSV columns.

- [x] **Step 3: Rebuild SQLite and write the evidence report**

Use the existing importer/database builder path to rebuild `XC2SaveNETThingy/XC2Data.db`, then write Markdown and JSON reports containing each marked ID, item type, English/Chinese/Japanese evidence, and reason.

- [x] **Step 4: Run the schema/data check**

Verify that `ItemStatus.csv` has 19 unique IDs, every status is `Reserved`, every ID exists in `Items`, and the rebuilt database contains the same records.

- [ ] **Step 5: Commit the data layer**

```powershell
git add XC2SaveNETThingy/db/Items_ItemStatus.sql XC2SaveNETThingy/data/ItemStatus.csv XC2SaveNETThingy/XC2Data.db reports/xc2-reserved-items.md reports/xc2-reserved-items.json
git commit -m "feat(data): mark reserved XC2 items"
```

### Task 2: Expose Reserved status through shared item queries

**Files:**
- Modify: `XC2SaveNETThingy/XC2Data.cs`
- Test: `tools/XC2BdatImporter/DatabaseSelfTests.cs`

- [x] **Step 1: Write the failing query contract check**

Add importer persistence coverage, then validate the runtime query contract against the rebuilt database: item tables expose `Reserved=1` and display text containing `[Reserved]` for seeded candidates.

- [x] **Step 2: Implement the query changes**

Update all item category queries to return `ID`, display `Name`, and `Reserved`; append `[Reserved]` only when the value is nonzero. Keep legacy fallback behavior only if an old external database lacks the column.

- [x] **Step 3: Run the focused check**

```powershell
dotnet build .\XC2SaveNETThingy\XC2SaveNETThingy.csproj -c Release --no-restore
```

Expected: build succeeds.

- [ ] **Step 4: Commit the shared query layer**

```powershell
git add XC2SaveNETThingy/XC2Data.cs tools/XC2BdatImporter/DatabaseSelfTests.cs
git commit -m "feat(data): expose reserved item status"
```

### Task 3: Add prominent warnings to both editors

**Files:**
- Modify: `XC2SaveNETThingy/DataGridViewColumnFactory.cs`
- Modify: `XC2Bf2SaveNETThingy/frmMain.cs`
- Modify: `XC2IraSaveNETThingy/frmMain.cs`

- [x] **Step 1: Write the failing UI contract check**

Check that both editors call `ApplyReservedItemStyle` from their item-grid cell formatting handler; the check must fail before the calls are present.

- [x] **Step 2: Implement the warning style**

Apply `MistyRose`/`Maroon` row colors, white selected text on `IndianRed`, and a tooltip explaining that the item is retained/invalid game data. Clear the style for normal rows.

- [ ] **Step 3: Run the focused UI build**

```powershell
dotnet build .\XC2Bf2SaveNETThingy\XC2Bf2SaveNETThingy.csproj -c Release --no-restore
dotnet build .\XC2IraSaveNETThingy\XC2IraSaveNETThingy.csproj -c Release --no-restore
```

Expected: both builds succeed.

- [ ] **Step 4: Commit the UI warning**

```powershell
git add XC2SaveNETThingy/DataGridViewColumnFactory.cs XC2Bf2SaveNETThingy/frmMain.cs XC2IraSaveNETThingy/frmMain.cs
git commit -m "feat(ui): highlight reserved items"
```

### Task 4: Full verification and documentation

**Files:**
- Modify: `README.md`
- Modify: `CHANGELOG.md`
- Modify: `tools/README.md`

- [ ] **Step 1: Run all verification commands**

```powershell
dotnet run --project .\tools\XC2BdatImporter -- --self-test
dotnet run --project .\tools\XC2SaveRoundTrip -- --self-test
dotnet build .\XC2SaveNETThingy.slnx -c Release --no-restore
dotnet build .\XC2SaveNETThingy.sln -c Release --no-restore
git diff --check
```

- [ ] **Step 2: Smoke-test both application executables**

Start both Release executables, confirm each remains alive for 5 seconds, then terminate only the test process.

- [ ] **Step 3: Document the status semantics**

Document that `Reserved` entries remain selectable, are marked in red, and are not evidence that the save format rejects the ID.

- [ ] **Step 4: Commit documentation**

```powershell
git add README.md CHANGELOG.md tools/README.md
git commit -m "docs: explain reserved item warnings"
```
