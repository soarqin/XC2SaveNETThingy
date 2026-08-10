# Tab 列表批量刷新设计

**日期：** 2026 年 8 月 10 日

## 背景

XC2 与 Torna 编辑器的部分 tab 打开速度较慢。两套编辑器分别位于 `XC2Bf2SaveNETThingy/frmMain.cs` 和 `XC2IraSaveNETThingy/frmMain.cs`，列表和表格的加载结构基本一致。

当前代码同时使用 `BindingList<T>`、`DataGridView.DataSource`、`ComboBox.DataSource` 和 `Items.Add()`。不同 tab 的慢点并不完全相同，但已绑定列表在逐项修改时会产生多次数据通知，数据绑定控件可能随每次通知重建行或触发关联事件。

## 目标

- 覆盖 XC2 与 Torna 两个编辑器。
- 批量填充列表时不触发逐项 UI 更新。
- 每个列表完成填充后只执行一次最终刷新。
- 保留现有数据顺序、控件绑定关系和最终选中项。
- 不改变保存文件格式、编辑语义和 tab 的可见结构。
- 为后续处理仍然较慢的特殊 tab 保留可测量的边界。

## 非目标

- 本轮不把所有 tab 改为首次选中时延迟加载。
- 本轮不重写现有 WinForms 窗体结构。
- 本轮不通过全局禁用事件或屏蔽 warning 的方式隐藏问题。

## 现状判断

`SetupFlagEditors()` 先将数据加入新的 `BindingList<T>`，再把列表绑定到 `DataGridView`，因此这一段不是典型的「绑定后逐行添加」。`SetupItemBox()` 也会先构造 `BindingList<Item>`，再设置 `DataSource`。

需要重点检查和改造的边界是：

1. 已设置 `DataSource` 后继续修改列表的代码。
2. 反复执行 `DataSource = null`、修改列、重新设置 `DataSource` 的代码。
3. 批量设置 `ComboBox` 或 `ListBox` 数据时触发的选择变化事件。
4. tab 选择事件中对多个关联控件重复绑定的代码。

## 设计

### 1. 批量修改已绑定列表

对需要在绑定后追加数据的 `BindingList<T>`，采用以下顺序：

```csharp
bool raiseListChangedEvents = list.RaiseListChangedEvents;
list.RaiseListChangedEvents = false;
try
{
    foreach (T item in items)
        list.Add(item);
}
finally
{
    list.RaiseListChangedEvents = raiseListChangedEvents;
}

if (raiseListChangedEvents)
    list.ResetBindings();
```

如果列表尚未绑定，则直接构造完整集合后再设置 `DataSource`，不额外触发刷新。

### 2. 控件刷新边界

涉及多个控件的 tab 加载过程使用局部刷新边界：

- 加载开始前保存当前选中项和必要的绑定状态。
- 暂停控件布局和列表控件的逐项重绘。
- 一次性完成数据集合、列和 `DataSource` 设置。
- 恢复布局和重绘。
- 只触发一次最终选择同步或显式刷新。

不得使用跨 tab 的全局静态加载标志。需要抑制事件时，在对应窗体或加载方法内使用作用域明确的状态，并通过 `try/finally` 保证异常时恢复状态。

### 3. 两套编辑器保持同构

先在两份 `frmMain.cs` 中找到结构相同的加载方法，再采用相同的批量刷新模式。若某个 tab 存在不同的数据结构或事件依赖，保留局部实现，不强行抽象成通用控件框架。

### 4. 性能记录

只在需要定位的加载边界记录耗时，不在每一行数据上记录日志。记录内容包括：

- 编辑器类型。
- tab 或加载方法名称。
- 数据项数量。
- 数据绑定前后耗时。

计时记录应便于开发阶段比较，避免改变正式 UI 文案或保存文件行为。

## 兼容性要求

- 数据项顺序不变。
- 当前选中索引在仍然有效时保持不变；无效时使用控件原有默认选择行为。
- `SelectedIndexChanged` 等事件不能因为中间状态被永久跳过；加载结束后必须完成一次有效同步。
- 添加、删除和编辑 Item 的现有行为保持不变。
- 两个编辑器都必须可在 Debug 和 Release 配置下编译。

## 验证方式

### 自动验证

- 为批量列表更新逻辑增加最小回归覆盖，确认多项数据只产生一次最终刷新通知。
- 构建完整解决方案：

```powershell
dotnet build .\XC2SaveNETThingy.sln --configuration Debug --no-restore --nologo -warnaserror
dotnet build .\XC2SaveNETThingy.sln --configuration Release --no-restore --nologo -warnaserror
```

预期结果为 0 个 warning、0 个 error。

### 手工验证

- 分别打开 XC2 与 Torna 存档。
- 进入原本较慢的 flags、items、drivers、blades 和其他数据 tab。
- 核对列表数量、显示文本、选中项和编辑操作。
- 对比优化前后的 tab 首次打开耗时和重复打开耗时。
- 确认切换语言、切换 item 子 tab、添加 Item 等关联事件仍然正常。

## 变更边界

预计修改：

- `XC2Bf2SaveNETThingy/frmMain.cs`
- `XC2IraSaveNETThingy/frmMain.cs`
- 如有必要，新增或修改核心的批量列表更新辅助代码及其测试。

不修改：

- 存档数据结构。
- 数据库内容和 CSV 数据。
- Designer 文件，除非验证确认控件初始化本身是性能瓶颈。
