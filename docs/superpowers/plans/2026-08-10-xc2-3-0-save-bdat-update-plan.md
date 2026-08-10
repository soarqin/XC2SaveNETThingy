# XC2 3.0 存档兼容与 BDAT 数据更新实施计划

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development（推荐）或 superpowers:executing-plans 执行本计划。每一步使用复选框跟踪，并在独立提交中完成可验证的逻辑单元。

**Goal:** 使用 3.0 BDAT 更新本篇与 Torna 编辑器的共享数据，并为 2.x/3.0 本篇与 Torna 存档提供可验证的无损 round-trip 兼容。

**Architecture:** 新增一个离线 .NET 10 控制台生成器，调用 bdat-toolset 提取 JSON，通过 common.bdat 的 Name 索引关联 3.0U/bdat/gb 英文文本，再以现有 SQL DDL 和规范 CSV 生成候选 XC2Data.db。共享存档模型只做已确认的版本字段命名和严格长度/魔数校验；两个 WinForms 应用继续从旁边的 XC2Data.db 读取数据。

**Tech Stack:** C#、.NET 10 控制台与 WinForms、System.Text.Json、现有 System.Data.SQLite.Core 1.0.109.2、PowerShell、bdat-toolset 0.6.0、ZLIB。

---

## 文件结构与职责

计划新增或修改的文件按职责划分：

- tools/XC2BdatImporter/XC2BdatImporter.csproj：离线 BDAT 生成器项目。
- tools/XC2BdatImporter/Program.cs：参数解析、阶段编排和退出码。
- tools/XC2BdatImporter/BdatToolsetRunner.cs：启动 bdat-toolset 并检查结果。
- tools/XC2BdatImporter/BdatJsonReader.cs、BdatModels.cs：读取 schema 和 rows JSON。
- tools/XC2BdatImporter/MappingConfig.cs、mappings.json、NameJoiner.cs：表映射、Name 索引关联和 ID 偏移。
- tools/XC2BdatImporter/ProjectDataWriter.cs：生成 UTF-16 LE、TAB 分隔的项目 CSV。
- tools/XC2BdatImporter/DatabaseBuilder.cs：由 db/*.sql 和 CSV 生成候选 SQLite。
- tools/XC2BdatImporter/DiffReportWriter.cs、Verifier.cs：差异报告、完整性和安全替换。
- tools/XC2SaveRoundTrip/：引用共享存档类库的 round-trip 验证器。
- tools/README.md：记录生成器版本、输入目录、命令和样本格式。
- XC2SaveNETThingy/SaveObjects/XC2Save.cs：版本字段命名和边界校验。
- 两个 WinForms 项目的 frmMain.cs 与 frmMain.Designer.cs：更新版本字段标签。
- XC2SaveNETThingy/data/*.csv、XC2Data.db：生成器验证通过后替换。
- CHANGELOG.md、README.md：实现完成后补充 3.0 说明。

不得修改、暂存或提交 romdata/。不得把用户现有 SDK 迁移改动混入本次数据提交。

## Task 1：建立生成器项目和命令行骨架

**Files**

- Create: tools/XC2BdatImporter/XC2BdatImporter.csproj
- Create: tools/XC2BdatImporter/Program.cs
- Create: tools/XC2BdatImporter/BdatToolsetRunner.cs
- Modify: XC2SaveNETThingy.slnx、XC2SaveNETThingy.sln

- [ ] Step 1：创建控制台项目，使用 net10.0，关闭隐式 using 和 nullable，并引用 System.Data.SQLite.Core 1.0.109.2。
- [ ] Step 2：让 Program.Main 接受 --base-bdat、--base-gb、--torna-bdat、--torna-gb、--toolset、--output、--report、--verify-only、--replace、--self-test。缺少参数返回 2，输入或验证失败返回 1，成功返回 0。
- [ ] Step 3：实现 BdatToolsetRunner，使用 ProcessStartInfo.ArgumentList 调用 extract --out-dir <dir> --file-type json --pretty <input>，捕获标准输出和标准错误；非零退出码抛出包含输入路径的异常。所有提取目录写入系统临时目录。
- [ ] Step 4：将项目加入 .slnx 和旧 .sln。
- [ ] Step 5：运行 dotnet restore .\XC2SaveNETThingy.slnx --force-evaluate 与 dotnet build .\tools\XC2BdatImporter\XC2BdatImporter.csproj --configuration Release --no-restore，确认生成器编译通过。
- [ ] Step 6：提交 feat: add XC2 BDAT importer skeleton，并确认暂存列表不包含 romdata/、三个 csproj 或 packages.config。

## Task 2：读取 BDAT JSON 并验证结构

**Files**

- Create: tools/XC2BdatImporter/BdatJsonReader.cs
- Create: tools/XC2BdatImporter/BdatModels.cs
- Create: tools/XC2BdatImporter/ImporterSelfTests.cs
- Modify: tools/XC2BdatImporter/Program.cs

- [ ] Step 1：定义支持 schema、rows、$id 和 JsonElement 的 JSON 模型。
- [ ] Step 2：实现 BdatJsonReader.ReadTable，检查文件存在、JSON 可解析、schema 非空、必需列存在和每行字段类型一致；缺少列或重复 $id 时抛出包含文件、表和字段的 BdatSchemaException。
- [ ] Step 3：加入 --self-test，覆盖正常表、缺少列、重复 $id、null 字符串和未知字段，成功输出 SELF_TEST_OK。
- [ ] Step 4：用真实 gb/chr_dr_ms.bdat 做烟测；预期识别 21 行以及 style、name 字段。
- [ ] Step 5：提交 feat: parse BDAT JSON tables。

## Task 3：编写表映射和 Name 索引关联

**Files**

- Create: tools/XC2BdatImporter/MappingConfig.cs
- Create: tools/XC2BdatImporter/mappings.json
- Create: tools/XC2BdatImporter/NameJoiner.cs
- Create: tools/XC2BdatImporter/MappingSelfTests.cs

- [ ] Step 1：定义 sourceFile、sourceTable、localizedFile、localizedTable、targetCsv、targetTable、keyColumn、localizedKeyColumn、idExpression、itemTypeId、preserveExistingRows 和 filter 配置字段。
- [ ] Step 2：覆盖 Drivers、DriverArts、DriverSkills、WeaponTypes、Weapons、BladeArts、BladeSkills、BladeSpecials、BladeSpecialsLv4、BladeFieldSkills、Elements、BladeTrustRanks、所有现有物品类别和 Poppiswap 类别；通用数据来自 common.bdat，英文名称来自 gb。
- [ ] Step 3：显式记录项目 ID 偏移 10000、15000、17000、20000、25000、26000、30000、35000、40000、45000、50000、56000–60000 以及约 5000 的武器 ID 空间；禁止根据 CSV 行号推导项目 ID。
- [ ] Step 4：实现 NameJoiner，先读取通用记录的原始 Name unsigned-short 索引，再查找本地化表；拒绝重复索引、缺失索引和未知 [ML:*] 标记，只允许 [ML:Space] 归一化为空格。
- [ ] Step 5：加入 Rapid Attack、Initiative、非顺序必杀技和 Team[ML:Space] 清理自检。
- [ ] Step 6：提交 feat: map XC2 3.0 tables by name index。

## Task 4：生成项目 CSV 和候选 SQLite

**Files**

- Create: tools/XC2BdatImporter/ProjectDataWriter.cs
- Create: tools/XC2BdatImporter/DatabaseBuilder.cs
- Create: tools/XC2BdatImporter/DatabaseSelfTests.cs

- [ ] Step 1：实现 UTF-16 LE、TAB 分隔 CSV 读写，忽略空行和 // 注释，保留表头、注释区和字段顺序。
- [ ] Step 2：按文件名排序执行 28 个 DDL，再按表头生成参数化 INSERT；使用单一事务，失败回滚且不保留半成品数据库。
- [ ] Step 3：从当前版本复制 FrmAboutText、FrmFAQText、FrmMainText、FrmMainInternalText、Flags、FavCategoriesIra 和其他不在 BDAT 映射中的人工 CSV，不使用游戏文本覆盖。
- [ ] Step 4：测试含制表符、空字符串、单引号和 Unicode 的临时数据，确认 CSV 与 SQLite 逐字段一致。
- [ ] Step 5：提交 feat: generate XC2 project data and SQLite。

## Task 5：差异报告、完整性检查和安全替换

**Files**

- Create: tools/XC2BdatImporter/DiffReportWriter.cs
- Create: tools/XC2BdatImporter/Verifier.cs
- Create: tools/XC2BdatImporter/VerifySelfTests.cs
- Modify: tools/XC2BdatImporter/Program.cs

- [ ] Step 1：固定 JSON/Markdown 报告字段：table、oldRowCount、newRowCount、addedIds、removedIds、changedIds、preservedRows、filteredRows、warnings。
- [ ] Step 2：执行 PRAGMA integrity_check、主键唯一、必需 ID、CSV/SQLite 逐字段比较；读取 PRAGMA foreign_key_check 作为约 1811 条违规基线，要求本次不增加。
- [ ] Step 3：比较 3.0 与 3.0U 的 common.bdat、common_gmk.bdat、lookat.bdat SHA-256；Torna 目录只校验语言文件集合。
- [ ] Step 4：只有显式指定 --replace 且候选库全部通过时才替换 CSV 和 XC2Data.db；先备份旧文件并记录 SHA-256。
- [ ] Step 5：使用正确库、重复主键库和坏字段库验证通过/失败路径。
- [ ] Step 6：提交 feat: validate and report XC2 data updates。

## Task 6：命名存档版本字段并强化序列化边界

**Files**

- Modify: XC2SaveNETThingy/SaveObjects/XC2Save.cs
- Modify: XC2Bf2SaveNETThingy/frmMain.cs、frmMain.Designer.cs
- Modify: XC2IraSaveNETThingy/frmMain.cs、frmMain.Designer.cs

- [ ] Step 1：将 Unk_0x117390 改为 SaveVersion，继续读写 784 字节原始区域，并增加 SaveVersionValue => BitConverter.ToUInt16(SaveVersion, 0)。必要时暂留 Obsolete 转发属性。
- [ ] Step 2：在 Deserialize 读取压缩头前检查输入至少 16 字节；解压后检查长度再读取魔数；错误消息显示十六进制期望长度和实际长度。
- [ ] Step 3：将两个窗体的 tabUnk_0x117390.Text 改为 SaveVersion，HexBox 字段同步为 hbxSaveVersion，不扩大长度、不限制取值、不自动升级。
- [ ] Step 4：分别构建共享库、本篇和 Torna Release 项目。
- [ ] Step 5：提交 fix: name XC2 save version field。

## Task 7：增加存档 round-trip 验证器

**Files**

- Create: tools/XC2SaveRoundTrip/XC2SaveRoundTrip.csproj
- Create: tools/XC2SaveRoundTrip/Program.cs
- Create: tools/XC2SaveRoundTrip/RawSaveInspector.cs
- Modify: XC2SaveNETThingy.slnx、XC2SaveNETThingy.sln

- [ ] Step 1：创建 net10.0 控制台项目，引用 XC2SaveNETThingy.csproj 和 ZLIB.dll；参数为 --input-dir、--output-dir、--compress。
- [ ] Step 2：对每个样本执行 Deserialize、Serialize、再次 Deserialize，报告压缩前后长度、魔数、SaveVersionValue、模型类型和失败原因。
- [ ] Step 3：比较解压后的文件头、Drivers、Blades、ItemBox、ContentVersions、SaveVersion 和整体长度；允许 ZLIB 压缩字节变化，不允许结构区域变化。
- [ ] Step 4：提交 test: add XC2 save round-trip verifier。

## Task 8：运行 3.0 BDAT 导入并审阅差异

**Files**

- Modify: XC2SaveNETThingy/data/*.csv（仅映射表）
- Modify: XC2SaveNETThingy/XC2Data.db
- Create: reports/xc2-3.0-data-diff.json、reports/xc2-3.0-data-diff.md
- Modify: tools/README.md

- [ ] Step 1：使用四份资源和 bdat-toolset 生成候选库，先使用 --verify-only，不改变项目文件。
- [ ] Step 2：审阅 ITM_OrbEquip 的 426 条记录、关键道具合并、Guest/DUMMY、武器约 5000 ID 空间、menu_attr_ms 兼容值和 [ML:Space] 清理；发现删除、既有 ID 重映射或新增外键违规时停止。
- [ ] Step 3：审阅通过后显式增加 --replace，记录替换前后 CSV 与 XC2Data.db SHA-256。
- [ ] Step 4：运行 git status --short、git diff --check 和针对 data、数据库、reports、tools 的 diff，确认 romdata/ 没有进入暂存区。
- [ ] Step 5：提交 feat: update XC2 data for version 3.0。

## Task 9：构建、启动和文档验证

**Files**

- Modify: CHANGELOG.md
- Modify: README.md

- [ ] Step 1：运行 net10.0 的 slnx 和旧 sln Release 构建。
- [ ] Step 2：比较两个输出目录的 XC2Data.db SHA-256 与源码数据库。
- [ ] Step 3：分别启动本篇和 Torna Release EXE 5 秒，确认没有立即退出；只结束本次启动的进程。
- [ ] Step 4：执行生成器 --verify-only 和 XC2SaveRoundTrip；没有真实样本时只报告结构级验证。
- [ ] Step 5：记录 3.0 数据来源、英文名称来源、Torna 文本边界、存档布局和生成命令。
- [ ] Step 6：提交 docs: document XC2 3.0 compatibility。

## 验收命令

~~~powershell
dotnet run --project .\tools\XC2BdatImporter -- --self-test
dotnet build .\XC2SaveNETThingy.slnx --configuration Release --no-restore --verbosity minimal
dotnet build .\XC2SaveNETThingy.sln --configuration Release --no-restore --verbosity minimal
dotnet run --project .\tools\XC2SaveRoundTrip -- --input-dir <external-samples> --output-dir <temp-output> --compress
git diff --check
~~~

预期结果：生成器自检、两个解决方案构建和可提供的存档 round-trip 检查均通过；差异报告和最终工作树状态可追溯。
