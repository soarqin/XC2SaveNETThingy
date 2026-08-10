# Tab 列表批量刷新实现计划

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** 在 XC2 与 Torna 编辑器中消除 flags 列表加载期间的逐行行头重算，使每个 flags 表在批量绑定结束后只进行一次完整行头刷新。

**Architecture:** 保留现有 `DataGridView`、`BindingList<T>` 和事件绑定关系。在窗体实例内增加局部的 flags 加载状态；加载期间让 `RowsAdded` 事件直接返回，加载结束后统一设置六个表的行头。非加载期间的 `RowsAdded` 事件只处理事件提供的新行范围，避免再次遍历整张表。flags 集合使用共享的 `BindingList<T>.AddRange` 批量填充辅助方法，保证集合填充不会产生逐项 `ListChanged` 通知。

**Tech Stack:** C#、.NET 10 Windows Forms、`DataGridView`、`BindingList<T>`、现有 `tools/XC2SaveRoundTrip` self-test。

---

### Task 1: 增加批量填充辅助逻辑及回归 self-test

**Files:**
- Create: `XC2SaveNETThingy/BindingListExtensions.cs`
- Modify: `tools/XC2SaveRoundTrip/RoundTripSelfTests.cs`

- [ ] **Step 1: 先写失败的 self-test**

在 `RoundTripSelfTests.Run()` 末尾增加 `BindingListAddRangeProducesOneResetNotification();`，并在类中增加以下测试代码：

```csharp
private static void BindingListAddRangeProducesOneResetNotification()
{
    BindingList<int> values = new BindingList<int>();
    int notificationCount = 0;
    ListChangedType lastChangeType = ListChangedType.ItemAdded;
    values.ListChanged += (sender, e) =>
    {
        notificationCount++;
        lastChangeType = e.ListChangedType;
    };

    values.AddRange(new[] { 10, 20, 30 });

    AssertEqual(3, values.Count, "batch list item count");
    AssertEqual(1, notificationCount, "batch list notification count");
    if (lastChangeType != ListChangedType.Reset)
        throw new InvalidOperationException("batch list notification was not Reset.");
}
```

同时补充文件头引用：

```csharp
using System.ComponentModel;
```

- [ ] **Step 2: 运行失败测试，确认失败原因是辅助方法不存在**

运行：

```powershell
dotnet build .\tools\XC2SaveRoundTrip\XC2SaveRoundTrip.csproj --configuration Debug --no-restore --nologo
```

预期：构建失败，错误指向 `BindingList<int>` 没有 `AddRange` 扩展方法；不能接受由语法错误或环境缺失导致的失败。

- [ ] **Step 3: 实现最小批量填充辅助方法**

创建 `XC2SaveNETThingy/BindingListExtensions.cs`，内容为：

```csharp
using System.Collections.Generic;
using System.ComponentModel;

namespace XC2SaveNETThingy
{
    public static class BindingListExtensions
    {
        public static void AddRange<T>(this BindingList<T> list, IEnumerable<T> items)
        {
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
        }
    }
}
```

- [ ] **Step 4: 运行 self-test，确认批量填充只产生一次最终通知**

运行：

```powershell
dotnet run --project .\tools\XC2SaveRoundTrip\XC2SaveRoundTrip.csproj --configuration Debug --no-build -- --self-test
```

预期输出包含 `SELF_TEST_OK`，退出码为 `0`。

### Task 2: 优化 XC2 flags 编辑器的批量绑定和行头刷新

**Files:**
- Modify: `XC2Bf2SaveNETThingy/frmMain.cs:15-30, 2127-2173, 4315-4330`

- [ ] **Step 1: 增加窗体实例级加载状态**

在 `#region Globals` 的现有布尔字段附近增加：

```csharp
private bool IsLoadingFlagEditors = false;
```

该状态不能声明为 `static`，也不能跨窗体共享。

- [ ] **Step 2: 让 `SetupFlagEditors()` 在绑定期间跳过逐项行头处理**

将六个 flags 列表的逐项 `Add` 改为 `AddRange`，并用 `try/finally` 包住完整加载过程：

```csharp
private void SetupFlagEditors()
{
    IsLoadingFlagEditors = true;
    try
    {
        BindingList<FlagData.Flag_1> Flags_1Bit_bl = new BindingList<FlagData.Flag_1>();
        Flags_1Bit_bl.AddRange(SAVEDATA.Flags.Flags_1Bit);
        dgvFlags1Bit = SetupFlagEditor(dgvFlags1Bit, Flags_1Bit_bl, "Value", "Set", true, 30);

        BindingList<FlagData.Flag_2_4_8> Flags_2Bit_bl = new BindingList<FlagData.Flag_2_4_8>();
        Flags_2Bit_bl.AddRange(SAVEDATA.Flags.Flags_2Bit);
        dgvFlags2Bit = SetupFlagEditor(dgvFlags2Bit, Flags_2Bit_bl, "Value", "Value", false, 40, 3);

        BindingList<FlagData.Flag_2_4_8> Flags_4Bit_bl = new BindingList<FlagData.Flag_2_4_8>();
        Flags_4Bit_bl.AddRange(SAVEDATA.Flags.Flags_4Bit);
        dgvFlags4Bit = SetupFlagEditor(dgvFlags4Bit, Flags_4Bit_bl, "Value", "Value", false, 40, 15);

        BindingList<FlagData.Flag_2_4_8> Flags_8Bit_bl = new BindingList<FlagData.Flag_2_4_8>();
        Flags_8Bit_bl.AddRange(SAVEDATA.Flags.Flags_8Bit);
        dgvFlags8Bit = SetupFlagEditor(dgvFlags8Bit, Flags_8Bit_bl, "Value", "Value", false, 40, 255);

        BindingList<FlagData.Flag_16> Flags_16Bit_bl = new BindingList<FlagData.Flag_16>();
        Flags_16Bit_bl.AddRange(SAVEDATA.Flags.Flags_16Bit);
        dgvFlags16Bit = SetupFlagEditor(dgvFlags16Bit, Flags_16Bit_bl, "Value", "Value", false, 55, 65535);

        BindingList<FlagData.Flag_32> Flags_32Bit_bl = new BindingList<FlagData.Flag_32>();
        Flags_32Bit_bl.AddRange(SAVEDATA.Flags.Flags_32Bit);
        dgvFlags32Bit = SetupFlagEditor(dgvFlags32Bit, Flags_32Bit_bl, "Value", "Value", false, 80, 4294967295);
    }
    finally
    {
        IsLoadingFlagEditors = false;
        FlagEditorRowHeaderSet(dgvFlags1Bit, XC2Data.Flags1Bit());
        FlagEditorRowHeaderSet(dgvFlags2Bit);
        FlagEditorRowHeaderSet(dgvFlags4Bit);
        FlagEditorRowHeaderSet(dgvFlags8Bit, XC2Data.Flags8Bit());
        FlagEditorRowHeaderSet(dgvFlags16Bit, XC2Data.Flags16Bit());
        FlagEditorRowHeaderSet(dgvFlags32Bit, XC2Data.Flags32Bit());
    }
}
```

- [ ] **Step 3: 将行头设置改为支持全量和增量范围**

把 `FlagEditorRowHeaderSet` 改为只遍历指定范围；不传范围时保持现有的全表行为：

```csharp
private void FlagEditorRowHeaderSet(
    DataGridView dgv,
    Dictionary<int, string> flagNames = null,
    int firstRowIndex = 0,
    int rowCount = -1)
{
    int first = Math.Max(0, firstRowIndex);
    int count = rowCount < 0 ? dgv.Rows.Count - first : rowCount;
    int last = Math.Min(dgv.Rows.Count, first + Math.Max(0, count));
    for (int index = first; index < last; index++)
    {
        DataGridViewRow row = dgv.Rows[index];
        if (flagNames == null || !flagNames.ContainsKey(index))
            row.HeaderCell.Value = String.Format("{0}", index + 1);
        else
            row.HeaderCell.Value = String.Format("{0}: {1}", index + 1, flagNames[index]);
    }
}
```

将六个 `RowsAdded` 处理器改为：

```csharp
private void dgvFlags1Bit_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
{
    if (!IsLoadingFlagEditors)
        FlagEditorRowHeaderSet((DataGridView)sender, XC2Data.Flags1Bit(), e.RowIndex, e.RowCount);
}
private void dgvFlags2Bit_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
{
    if (!IsLoadingFlagEditors)
        FlagEditorRowHeaderSet((DataGridView)sender, null, e.RowIndex, e.RowCount);
}
private void dgvFlags4Bit_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
{
    if (!IsLoadingFlagEditors)
        FlagEditorRowHeaderSet((DataGridView)sender, null, e.RowIndex, e.RowCount);
}
private void dgvFlags8Bit_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
{
    if (!IsLoadingFlagEditors)
        FlagEditorRowHeaderSet((DataGridView)sender, XC2Data.Flags8Bit(), e.RowIndex, e.RowCount);
}
private void dgvFlags16Bit_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
{
    if (!IsLoadingFlagEditors)
        FlagEditorRowHeaderSet((DataGridView)sender, XC2Data.Flags16Bit(), e.RowIndex, e.RowCount);
}
private void dgvFlags32Bit_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
{
    if (!IsLoadingFlagEditors)
        FlagEditorRowHeaderSet((DataGridView)sender, XC2Data.Flags32Bit(), e.RowIndex, e.RowCount);
}
```

- [ ] **Step 4: 重复同构修改到 Torna 编辑器**

在 `XC2IraSaveNETThingy/frmMain.cs:15-30, 1558-1604, 3324-3339` 应用同一批量加载和增量行头范围逻辑。Torna 的 `Flags_2Bit_bl`、`Flags_4Bit_bl`、`Flags_8Bit_bl`、`Flags_16Bit_bl`、`Flags_32Bit_bl` 参数和 XC2 保持一致；不修改 Designer 中的事件绑定。

- [ ] **Step 5: 运行两套编辑器的编译验证**

运行：

```powershell
dotnet build .\XC2SaveNETThingy.sln --configuration Debug --no-restore --nologo -warnaserror
dotnet build .\XC2SaveNETThingy.sln --configuration Release --no-restore --nologo -warnaserror
```

预期：两个配置均退出码为 `0`，输出均为 `0 个警告，0 个错误`。

### Task 3: 明确本轮优化边界并检查未改动的慢点

**Files:**
- Review only: `XC2SaveNETThingy/DataGridViewColumnFactory.cs`
- Review only: `XC2Bf2SaveNETThingy/frmMain.cs`
- Review only: `XC2IraSaveNETThingy/frmMain.cs`

- [ ] **Step 1: 确认 Item 表绑定仍然是绑定前构造完整集合**

保留 `SetupItemBox()` 中先构造 `BindingList<Item>`、再设置 `DataSource` 的顺序。本轮不把 `ApplyReservedItemStyle()` 的查表缓存和 `SetupComboBoxes()` 的多控件重绑混入 flags 修复，以避免改变 Item 编辑和语言切换语义。

- [ ] **Step 2: 记录后续优化入口**

在交付说明中列出 `ApplyReservedItemStyle()` 每次格式化扫描 Item `DataTable`、`SetupComboBoxes()` 多次 `BindDataTable()` 以及不同 tab 的独立绑定耗时，作为下一轮按实际计时继续处理的边界；本轮不宣称这些慢点已经解决。

### Task 4: 完成静态和手工验证清单

**Files:**
- Review only: `XC2Bf2SaveNETThingy/frmMain.cs`
- Review only: `XC2IraSaveNETThingy/frmMain.cs`

- [ ] **Step 1: 检查所有 flags 加载事件均有加载状态保护**

运行：

```powershell
rg -n "IsLoadingFlagEditors|RowsAdded|FlagEditorRowHeaderSet" XC2Bf2SaveNETThingy\frmMain.cs XC2IraSaveNETThingy\frmMain.cs
```

确认六个 flags 表在每套编辑器中均满足：绑定期间跳过事件，`finally` 中完成一次全量行头设置，普通增量添加使用 `e.RowIndex` 与 `e.RowCount`。

- [ ] **Step 2: 运行 self-test 和完整构建**

运行：

```powershell
dotnet run --project .\tools\XC2SaveRoundTrip\XC2SaveRoundTrip.csproj --configuration Debug --no-build -- --self-test
dotnet build .\XC2SaveNETThingy.sln --configuration Debug --no-restore --nologo -warnaserror
dotnet build .\XC2SaveNETThingy.sln --configuration Release --no-restore --nologo -warnaserror
```

预期：self-test 输出 `SELF_TEST_OK`；Debug 和 Release 均为 `0 个警告，0 个错误`。

- [ ] **Step 3: 手工核对编辑器行为**

分别启动 XC2 和 Torna 编辑器并打开存档，检查 flags、items、drivers、blades 以及其他原本较慢的 tab：列表数量、显示文本、初始选中项、编辑保存行为、语言切换、Item 子 tab 切换和新增 Item。对 flags tab 比较首次打开与重复打开耗时，并确认行头编号和已命名 flag 文本正确。
