# Reserved 道具标记设计

## 目标

依据 XC2 3.0 的多语言 BDAT 对照和项目已有道具分类，将确认不是正常可获得道具的条目标记为 `Reserved`，并在本体与 Torna 编辑器中使用明显警告显示。`Reserved` 道具仍然可选，存档读写格式不改变。

## 数据设计

`Items` 保留原有 ID、类型和名称字段。新增独立的 `ItemStatus` 表，以 `ItemID` 关联 `Items`，保存 `Status`、双语原因和证据字段。当前 `Status` 约束为 `Reserved`，只为确认的临时占位或无效游戏数据建立记录。独立表会随数据库重建保留，避免仅修改二进制 SQLite 导致重新导入时丢失。

候选判定遵循以下规则：

1. 至少两种语言的名称都明确表达内部、无效、测试、占位或未使用语义，才自动标记。
2. 仅有空名称、`UNKNOWN_n` 或语言顺序差异的条目不自动标记。
3. 由 `ITM_PreciousListIra` 等通用 `common.bdat` 表定义的 Torna 正常道具不因位于 DLC 表而标记。
4. 事件、任务、营地和对话文本不属于 `Items`，只作为调查证据，不导入道具数据库。

## 界面设计

共享 `XC2Data` 查询为物品结果附带 `Reserved` 列，并在显示名称中增加 `[Reserved]` 标记。两个编辑器的物品箱行在 Reserved 时使用浅红背景、深红文字和工具提示；状态不改变 ID、数量或存档写入逻辑。

## 验证

- 数据库重建后，`ItemStatus` 表存在且包含 19 条 `Reserved` 记录。
- 每个标记 ID 都存在于 `Items`，不存在孤立状态记录。
- 本体和 Torna Release 构建通过，两个应用启动烟测通过。
- 现有 importer、round-trip 自检和 `git diff --check` 继续通过。
