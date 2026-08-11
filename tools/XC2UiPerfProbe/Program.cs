using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using XC2Bf2SaveNETThingy;
using XC2SaveNETThingy;

namespace XC2UiPerfProbe
{
    internal static class Program
    {
        private static StreamWriter log;
        private static Exception uiThreadException;
        private static Dictionary<string, int> handleTypeCounts;
        private static int handleCount;

        [STAThread]
        private static int Main(string[] args)
        {
            Environment.CurrentDirectory = AppContext.BaseDirectory;
            log = new StreamWriter(Path.Combine(AppContext.BaseDirectory, "XC2UiPerfProbe.log"), false);
            log.AutoFlush = true;
            Log("BASE=" + AppContext.BaseDirectory);
            Log("CWD=" + Environment.CurrentDirectory);
            bool databaseExists = File.Exists(Path.Combine(AppContext.BaseDirectory, "XC2Data.db"));
            Log("DB=" + databaseExists);
            Log("CFG=" + File.Exists(Path.Combine(AppContext.BaseDirectory, "settings.cfg")));

            if (!databaseExists)
            {
                Log("FATAL missing XC2Data.db beside probe executable");
                return 3;
            }

            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                AppDomain.CurrentDomain.UnhandledException += (sender, eventArgs) =>
                {
                    Exception exception = eventArgs.ExceptionObject as Exception ??
                        new Exception(Convert.ToString(eventArgs.ExceptionObject));
                    Log("UNHANDLED_EXCEPTION isTerminating=" + eventArgs.IsTerminating + " type=" + exception.GetType().FullName);
                    Log(exception.ToString());
                };
                TaskScheduler.UnobservedTaskException += (sender, eventArgs) =>
                {
                    Log("UNOBSERVED_TASK_EXCEPTION type=" + eventArgs.Exception.GetType().FullName);
                    Log(eventArgs.Exception.ToString());
                    eventArgs.SetObserved();
                };
                Application.ThreadException += (sender, eventArgs) =>
                {
                    uiThreadException = eventArgs.Exception;
                    Log("UI_THREAD_EXCEPTION type=" + eventArgs.Exception.GetType().FullName);
                    Log(eventArgs.Exception.ToString());
                };

                string savePath = Path.Combine(Path.GetTempPath(), "XC2UiPerfProbe.sav");
                byte[] saveData = new byte[XC2Save.SIZE];
                Buffer.BlockCopy(XC2Save.CORRECT_MAGIC, 0, saveData, 0, XC2Save.CORRECT_MAGIC.Length);
                File.WriteAllBytes(savePath, saveData);

                using (frmMain form = new frmMain())
                {
                    if (Environment.GetEnvironmentVariable("XC2_PERF_DISABLE_AUTOSCALE") == "1")
                        DisableAutoScale(form);

                    form.Show();
                    PumpMessages();

                    if (Environment.GetEnvironmentVariable("XC2_PERF_EMPTY_TABS") == "1")
                    {
                        TabControl emptyMainTabControl = FindControl<TabControl>(form, "tbcMain");
                        for (int pageIndex = 0; pageIndex < emptyMainTabControl.TabPages.Count; pageIndex++)
                        {
                            Stopwatch emptyWatch = Stopwatch.StartNew();
                            emptyMainTabControl.SelectedIndex = pageIndex;
                            PumpMessages();
                            emptyWatch.Stop();
                            Log("EMPTY_TAB index=" + pageIndex + " page=" + emptyMainTabControl.TabPages[pageIndex].Name + " ms=" + emptyWatch.Elapsed.TotalMilliseconds.ToString("F1"));
                        }
                        return 0;
                    }

                    if (Environment.GetEnvironmentVariable("XC2_PERF_HIDDEN_CREATE") == "1")
                    {
                        TabControl hiddenMainTabControl = FindControl<TabControl>(form, "tbcMain");
                        for (int pageIndex = 1; pageIndex < hiddenMainTabControl.TabPages.Count; pageIndex++)
                        {
                            TabPage page = hiddenMainTabControl.TabPages[pageIndex];
                            Stopwatch hiddenWatch = Stopwatch.StartNew();
                            page.CreateControl();
                            page.PerformLayout();
                            PumpMessages();
                            hiddenWatch.Stop();
                            Log("HIDDEN_CREATE index=" + pageIndex + " page=" + page.Name + " ms=" + hiddenWatch.Elapsed.TotalMilliseconds.ToString("F1"));
                        }
                        return 0;
                    }

                    if (Environment.GetEnvironmentVariable("XC2_PERF_DESCENDANT_CREATE") == "1")
                    {
                        TabControl descendantMainTabControl = FindControl<TabControl>(form, "tbcMain");
                        TabPage[] pagesToCreate =
                        {
                            descendantMainTabControl.TabPages[1],
                            descendantMainTabControl.TabPages[2],
                            descendantMainTabControl.TabPages[6]
                        };
                        foreach (TabPage page in pagesToCreate)
                        {
                            List<Control> descendants = new List<Control>();
                            CollectControls(page, descendants);
                            Stopwatch descendantWatch = Stopwatch.StartNew();
                            foreach (Control control in descendants)
                                control.CreateControl();
                            PumpMessages();
                            descendantWatch.Stop();
                            Log("DESCENDANT_CREATE page=" + page.Name + " controls=" + descendants.Count + " ms=" + descendantWatch.Elapsed.TotalMilliseconds.ToString("F1"));
                        }
                        Stopwatch descendantSelectWatch = Stopwatch.StartNew();
                        descendantMainTabControl.SelectedIndex = 1;
                        PumpMessages();
                        descendantSelectWatch.Stop();
                        Log("DESCENDANT_SELECT page=" + descendantMainTabControl.TabPages[1].Name + " ms=" + descendantSelectWatch.Elapsed.TotalMilliseconds.ToString("F1"));
                        return 0;
                    }

                    if (Environment.GetEnvironmentVariable("XC2_PERF_OFFSCREEN_WARMUP") == "1")
                    {
                        TabControl offscreenMainTabControl = FindControl<TabControl>(form, "tbcMain");
                        using (Form host = new Form
                        {
                            ShowInTaskbar = false,
                            FormBorderStyle = FormBorderStyle.None,
                            StartPosition = FormStartPosition.Manual,
                            Location = new System.Drawing.Point(-32000, -32000),
                            Size = form.ClientSize
                        })
                        {
                            TabControl warmupHostTabControl = new TabControl { Dock = DockStyle.Fill };
                            host.Controls.Add(warmupHostTabControl);
                            host.Show();
                            PumpMessages();
                            foreach (int pageIndex in new[] { 1, 2, 6 })
                            {
                                TabPage page = offscreenMainTabControl.TabPages[pageIndex];
                                Stopwatch offscreenWatch = Stopwatch.StartNew();
                                offscreenMainTabControl.TabPages.Remove(page);
                                warmupHostTabControl.TabPages.Add(page);
                                warmupHostTabControl.SelectedTab = page;
                                PumpMessages();

                                if (Environment.GetEnvironmentVariable("XC2_PERF_OFFSCREEN_RECURSIVE") == "1")
                                {
                                    List<TabControl> nestedTabControls = new List<TabControl>();
                                    CollectNestedTabControls(page, nestedTabControls);
                                    foreach (TabControl nestedTabControl in nestedTabControls)
                                    {
                                        Stopwatch nestedWatch = Stopwatch.StartNew();
                                        TabControlPerformance.WarmUpAllPages(nestedTabControl);
                                        PumpMessages();
                                        nestedWatch.Stop();
                                        Log("OFFSCREEN_NESTED_WARMUP tab=" + nestedTabControl.Name + " ms=" + nestedWatch.Elapsed.TotalMilliseconds.ToString("F1"));
                                    }
                                }

                                warmupHostTabControl.TabPages.Remove(page);
                                offscreenMainTabControl.TabPages.Insert(pageIndex, page);
                                offscreenWatch.Stop();
                                Log("OFFSCREEN_WARMUP page=" + page.Name + " ms=" + offscreenWatch.Elapsed.TotalMilliseconds.ToString("F1"));
                            }
                            host.Hide();
                        }

                        Stopwatch offscreenSelectWatch = Stopwatch.StartNew();
                        offscreenMainTabControl.SelectedIndex = 1;
                        PumpMessages();
                        offscreenSelectWatch.Stop();
                        Log("OFFSCREEN_SELECT page=" + offscreenMainTabTabName(offscreenMainTabControl, 1) + " ms=" + offscreenSelectWatch.Elapsed.TotalMilliseconds.ToString("F1"));
                        if (Environment.GetEnvironmentVariable("XC2_PERF_OFFSCREEN_WARMUP_ONLY") == "1")
                            return 0;
                    }

                    if (Environment.GetEnvironmentVariable("XC2_PERF_WARMUP_BEFORE_LOAD") == "1")
                    {
                        TabControl warmupMainTabControl = FindControl<TabControl>(form, "tbcMain");
                        Stopwatch warmupWatch = Stopwatch.StartNew();
                        TabControlPerformance.WarmUpAllPages(warmupMainTabControl);
                        PumpMessages();
                        warmupWatch.Stop();
                        Log("WARMUP_BEFORE_LOAD_MS=" + warmupWatch.Elapsed.TotalMilliseconds.ToString("F1"));
                    }

                    if (Environment.GetEnvironmentVariable("XC2_PERF_TOP_WARMUP_BEFORE_LOAD") == "1")
                    {
                        TabControl topWarmupTabControl = FindControl<TabControl>(form, "tbcMain");
                        Stopwatch topWarmupWatch = Stopwatch.StartNew();
                        for (int pageIndex = 0; pageIndex < topWarmupTabControl.TabPages.Count; pageIndex++)
                        {
                            topWarmupTabControl.SelectedIndex = pageIndex;
                            TabPage page = topWarmupTabControl.TabPages[pageIndex];
                            page.CreateControl();
                            page.PerformLayout();
                            PumpMessages();
                        }
                        topWarmupTabControl.SelectedIndex = 0;
                        topWarmupWatch.Stop();
                        Log("TOP_WARMUP_BEFORE_LOAD_MS=" + topWarmupWatch.Elapsed.TotalMilliseconds.ToString("F1"));
                    }

                    if (Environment.GetEnvironmentVariable("XC2_PERF_HEAVY_WARMUP_BEFORE_LOAD") == "1")
                    {
                        TabControl heavyWarmupTabControl = FindControl<TabControl>(form, "tbcMain");
                        Stopwatch heavyWarmupWatch = Stopwatch.StartNew();
                        foreach (int pageIndex in new[] { 1, 2, 6 })
                        {
                            heavyWarmupTabControl.SelectedIndex = pageIndex;
                            PumpMessages();
                        }
                        heavyWarmupTabControl.SelectedIndex = 0;
                        PumpMessages();
                        heavyWarmupWatch.Stop();
                        Log("HEAVY_WARMUP_BEFORE_LOAD_MS=" + heavyWarmupWatch.Elapsed.TotalMilliseconds.ToString("F1"));
                    }

                    if (Environment.GetEnvironmentVariable("XC2_PERF_EMPTY_PROGRESSIVE_CREATE") == "1")
                    {
                        MeasureProgressiveCreate(form);
                        ThrowIfUiThreadFailed();
                        return 0;
                    }

                    Stopwatch loadWatch = Stopwatch.StartNew();
                    InvokePrivate(form, "LoadSaveData", savePath);
                    loadWatch.Stop();
                    PumpMessages();
                    Log("LOAD_MS=" + loadWatch.Elapsed.TotalMilliseconds.ToString("F1"));

                    if (Environment.GetEnvironmentVariable("XC2_PERF_LIGHTWEIGHT_COMBO_ITEMS") == "1")
                    {
                        ReplaceDataTableComboSources(form);
                        MeasureProgressiveCreate(form);
                        ThrowIfUiThreadFailed();
                        return 0;
                    }

                    if (Environment.GetEnvironmentVariable("XC2_PERF_UNBIND_LARGE_COMBOS") == "1")
                    {
                        UnbindLargeComboSources(form);
                        MeasureUnboundComboTabs(form);
                        ThrowIfUiThreadFailed();
                        return 0;
                    }

                    if (Environment.GetEnvironmentVariable("XC2_PERF_DEFERRED_COMBO_SMOKE") == "1")
                    {
                        MeasureDeferredComboSmoke(form);
                        ThrowIfUiThreadFailed();
                        return 0;
                    }

                    if (Environment.GetEnvironmentVariable("XC2_PERF_LOADED_DESCENDANT_CREATE") == "1")
                    {
                        MeasureLoadedDescendantCreate(form);
                        ThrowIfUiThreadFailed();
                        return 0;
                    }

                    if (Environment.GetEnvironmentVariable("XC2_PERF_LOADED_OFFSCREEN_CREATE") == "1")
                    {
                        MeasureLoadedOffscreenCreate(form);
                        ThrowIfUiThreadFailed();
                        return 0;
                    }

                    if (Environment.GetEnvironmentVariable("XC2_PERF_HIDE_BEFORE_SELECT") == "1")
                    {
                        MeasureHiddenBeforeSelect(form);
                        ThrowIfUiThreadFailed();
                        return 0;
                    }

                    if (Environment.GetEnvironmentVariable("XC2_PERF_HIDDEN_CREATE_AFTER_SELECT") == "1")
                    {
                        MeasureHiddenCreateAfterSelect(form);
                        ThrowIfUiThreadFailed();
                        return 0;
                    }

                    if (Environment.GetEnvironmentVariable("XC2_PERF_PROGRESSIVE_CREATE") == "1")
                    {
                        MeasureProgressiveCreate(form);
                        ThrowIfUiThreadFailed();
                        return 0;
                    }

                    if (Environment.GetEnvironmentVariable("XC2_PERF_POSTLOAD_WARMUP") == "1")
                    {
                        TabControl postLoadWarmupMain = FindControl<TabControl>(form, "tbcMain");
                        foreach (string pageName in new[] { "tabDrivers", "tabBlades", "tabMercGroups", "tabItems", "tabFlags", "tabTigerTiger", "tabEvents" })
                        {
                            Stopwatch pageWarmupWatch = Stopwatch.StartNew();
                            TabControlPerformance.WarmUpPagesOffscreen(
                                postLoadWarmupMain,
                                page => page.Name == pageName,
                                false);
                            PumpMessages();
                            pageWarmupWatch.Stop();
                            Log("POSTLOAD_WARMUP page=" + pageName + " ms=" + pageWarmupWatch.Elapsed.TotalMilliseconds.ToString("F1"));
                        }
                    }

                    if (Environment.GetEnvironmentVariable("XC2_PERF_HANDLE_PROFILE") == "1")
                        AttachHandleProfile(form);

                    if (Environment.GetEnvironmentVariable("XC2_PERF_TOP_WARMUP_AFTER_LOAD") == "1")
                    {
                        TabControl topWarmupAfterLoad = FindControl<TabControl>(form, "tbcMain");
                        Stopwatch topWarmupAfterLoadWatch = Stopwatch.StartNew();
                        for (int pageIndex = 0; pageIndex < topWarmupAfterLoad.TabPages.Count; pageIndex++)
                        {
                            topWarmupAfterLoad.SelectedIndex = pageIndex;
                            PumpMessages();
                        }
                        topWarmupAfterLoad.SelectedIndex = 0;
                        PumpMessages();
                        topWarmupAfterLoadWatch.Stop();
                        Log("TOP_WARMUP_AFTER_LOAD_MS=" + topWarmupAfterLoadWatch.Elapsed.TotalMilliseconds.ToString("F1"));
                    }

                    if (Environment.GetEnvironmentVariable("XC2_PERF_HEAVY_WARMUP_AFTER_LOAD") == "1")
                    {
                        TabControl heavyAfterLoadMain = FindControl<TabControl>(form, "tbcMain");
                        foreach (int pageIndex in new[] { 1, 2, 6 })
                        {
                            Stopwatch heavyPageWatch = Stopwatch.StartNew();
                            TabPage heavyPage = heavyAfterLoadMain.TabPages[pageIndex];
                            TabControlPerformance.WarmUpPagesInPlace(
                                heavyAfterLoadMain,
                                page => ReferenceEquals(page, heavyPage),
                                true);
                            PumpMessages();
                            heavyPageWatch.Stop();
                            Log("HEAVY_WARMUP_AFTER_LOAD page=" + heavyAfterLoadMain.TabPages[pageIndex].Name + " ms=" + heavyPageWatch.Elapsed.TotalMilliseconds.ToString("F1"));
                        }
                        heavyAfterLoadMain.SelectedIndex = 0;
                        PumpMessages();
                    }

                    if (Environment.GetEnvironmentVariable("XC2_PERF_TARGETED_WARMUP_AFTER_LOAD") == "1")
                    {
                        TabControl targetedMain = FindControl<TabControl>(form, "tbcMain");
                        foreach (string pageName in new[] { "tabDrivers", "tabBlades", "tabMercGroups" })
                        {
                            Stopwatch targetedPageWatch = Stopwatch.StartNew();
                            TabControlPerformance.WarmUpPagesOffscreen(
                                targetedMain,
                                page => page.Name == pageName,
                                false);
                            PumpMessages();
                            targetedPageWatch.Stop();
                            Log("TARGETED_WARMUP page=" + pageName + " ms=" + targetedPageWatch.Elapsed.TotalMilliseconds.ToString("F1"));
                        }

                        foreach (string tabName in new[] { "tbcDrivers", "tbcBlades" })
                        {
                            TabControl nested = FindControl<TabControl>(form, tabName);
                            Stopwatch targetedNestedWatch = Stopwatch.StartNew();
                            TabControlPerformance.WarmUpAllPages(nested);
                            PumpMessages();
                            targetedNestedWatch.Stop();
                            Log("TARGETED_NESTED_WARMUP tab=" + tabName + " ms=" + targetedNestedWatch.Elapsed.TotalMilliseconds.ToString("F1"));
                        }
                    }

                    if (Environment.GetEnvironmentVariable("XC2_PERF_HIERARCHY") == "1")
                    {
                        DumpControlTree(form, 0);
                        return 0;
                    }

                    if (Environment.GetEnvironmentVariable("XC2_PERF_ITEMS") == "1")
                    {
                        TabControl itemsTabControl = FindControl<TabControl>(form, "tbcItems");
                        Stopwatch itemsCreateWatch = Stopwatch.StartNew();
                        itemsTabControl.CreateControl();
                        itemsCreateWatch.Stop();
                        Log("ITEMS_CREATE_MS=" + itemsCreateWatch.Elapsed.TotalMilliseconds.ToString("F1"));
                        for (int pageIndex = 0; pageIndex < itemsTabControl.TabPages.Count; pageIndex++)
                        {
                            Stopwatch pageWatch = Stopwatch.StartNew();
                            itemsTabControl.SelectedIndex = pageIndex;
                            PumpMessages();
                            pageWatch.Stop();
                            Log("ITEM_PAGE index=" + pageIndex + " page=" + itemsTabControl.TabPages[pageIndex].Name + " ms=" + pageWatch.Elapsed.TotalMilliseconds.ToString("F1"));
                        }
                        return 0;
                    }

                    if (Environment.GetEnvironmentVariable("XC2_PERF_ITEMS_ROOT") == "1")
                    {
                        TabControl mainTabControl = FindControl<TabControl>(form, "tbcMain");
                        TabControl itemsTabControl = FindControl<TabControl>(form, "tbcItems");
                        itemsTabControl.Visible = false;
                        Stopwatch hiddenWatch = Stopwatch.StartNew();
                        mainTabControl.SelectedIndex = 3;
                        PumpMessages();
                        hiddenWatch.Stop();
                        Log("ITEMS_ROOT_HIDDEN_MS=" + hiddenWatch.Elapsed.TotalMilliseconds.ToString("F1"));
                        return 0;
                    }

                    if (Environment.GetEnvironmentVariable("XC2_PERF_ITEM_ROWS") == "1")
                    {
                        DataGridView itemGrid = FindControl<DataGridView>(form, "dgvPcWpnChipBox");
                        Stopwatch rowWatch = Stopwatch.StartNew();
                        itemGrid.RowCount = 200;
                        rowWatch.Stop();
                        Log("ITEM_ROWS_MS=" + rowWatch.Elapsed.TotalMilliseconds.ToString("F1"));
                        return 0;
                    }

                    if (Environment.GetEnvironmentVariable("XC2_PERF_FLAG_ROWS") == "1")
                    {
                        SetAndLogRows(form, "dgvFlags1Bit", 65536);
                        SetAndLogRows(form, "dgvFlags2Bit", 65536);
                        SetAndLogRows(form, "dgvFlags4Bit", 8192);
                        SetAndLogRows(form, "dgvFlags8Bit", 8192);
                        SetAndLogRows(form, "dgvFlags16Bit", 3072);
                        SetAndLogRows(form, "dgvFlags32Bit", 3336);
                        return 0;
                    }

                    if (Environment.GetEnvironmentVariable("XC2_PERF_TRUE_FIRST") == "1")
                    {
                        TabControl trueFirstMainTabControl = FindControl<TabControl>(form, "tbcMain");
                        MeasureTabTree(trueFirstMainTabControl, "");
                        PumpMessages();
                        Log("FAILURES=0");
                        return 0;
                    }

                    List<TabControl> tabControls = new List<TabControl>();
                    CollectTabControls(form, tabControls);
                    Log("TAB_CONTROL_COUNT=" + tabControls.Count);

                    int failures = 0;
                    foreach (TabControl tabControl in tabControls)
                    {
                        int originalIndex = tabControl.SelectedIndex;
                        Log("TAB_CONTROL name=" + tabControl.Name + " pages=" + tabControl.TabPages.Count);
                        for (int pageIndex = 0; pageIndex < tabControl.TabPages.Count; pageIndex++)
                        {
                            try
                            {
                                Stopwatch watch = Stopwatch.StartNew();
                                tabControl.SelectedIndex = pageIndex;
                                PumpMessages();
                                watch.Stop();
                                Log(
                                    "TAB name=" + tabControl.Name +
                                    " index=" + pageIndex +
                                    " page=" + tabControl.TabPages[pageIndex].Name +
                                    " title=" + tabControl.TabPages[pageIndex].Text +
                                    " ms=" + watch.Elapsed.TotalMilliseconds.ToString("F1"));
                            }
                            catch (Exception ex)
                            {
                                failures++;
                                Log(
                                    "TAB_ERROR name=" + tabControl.Name +
                                    " index=" + pageIndex +
                                    " page=" + tabControl.TabPages[pageIndex].Name +
                                    " type=" + ex.GetType().FullName +
                                    " message=" + ex.Message);
                            }
                        }

                        if (originalIndex >= 0 && originalIndex < tabControl.TabPages.Count)
                            tabControl.SelectedIndex = originalIndex;
                    }

                    PumpMessages();
                    Log("FAILURES=" + failures);
                    ThrowIfUiThreadFailed();
                    return failures == 0 ? 0 : 1;
                }
            }
            catch (Exception ex)
            {
                Log("FATAL type=" + ex.GetType().FullName);
                Log(ex.ToString());
                return 2;
            }
        }

        private static void Log(string message)
        {
            Console.WriteLine(message);
            log?.WriteLine(message);
        }

        private static void PumpMessages()
        {
            Application.DoEvents();
            ThrowIfUiThreadFailed();
        }

        private static void ThrowIfUiThreadFailed()
        {
            if (uiThreadException != null)
                throw new InvalidOperationException("WinForms raised an asynchronous UI exception.", uiThreadException);
        }

        private static void DisableAutoScale(Control control)
        {
            if (control is ContainerControl container)
                container.AutoScaleMode = AutoScaleMode.None;

            foreach (Control child in control.Controls)
                DisableAutoScale(child);
        }

        private static void CollectTabControls(Control parent, List<TabControl> result)
        {
            foreach (Control child in parent.Controls)
            {
                if (child is TabControl tabControl)
                {
                    result.Add(tabControl);
                    CollectTabControls(tabControl, result);
                }
                else
                {
                    CollectTabControls(child, result);
                }
            }
        }

        private static void MeasureTabTree(TabControl tabControl, string parentName)
        {
            for (int pageIndex = 0; pageIndex < tabControl.TabPages.Count; pageIndex++)
            {
                ResetHandleProfile();
                Stopwatch watch = Stopwatch.StartNew();
                Stopwatch selectWatch = Stopwatch.StartNew();
                tabControl.SelectedIndex = pageIndex;
                selectWatch.Stop();
                Stopwatch pumpWatch = Stopwatch.StartNew();
                PumpMessages();
                pumpWatch.Stop();
                watch.Stop();
                Log(
                    "TRUE_FIRST tab=" + tabControl.Name +
                    " index=" + pageIndex +
                    " page=" + tabControl.TabPages[pageIndex].Name +
                    " title=" + tabControl.TabPages[pageIndex].Text +
                    " ms=" + watch.Elapsed.TotalMilliseconds.ToString("F1") +
                    (Environment.GetEnvironmentVariable("XC2_PERF_SPLIT_TIMING") == "1"
                        ? " selectMs=" + selectWatch.Elapsed.TotalMilliseconds.ToString("F1") +
                          " pumpMs=" + pumpWatch.Elapsed.TotalMilliseconds.ToString("F1")
                        : string.Empty) +
                    HandleProfileText());

                List<TabControl> nestedTabControls = new List<TabControl>();
                CollectNestedTabControls(tabControl.TabPages[pageIndex], nestedTabControls);
                foreach (TabControl nestedTabControl in nestedTabControls)
                    MeasureTabTree(nestedTabControl, tabControl.Name);
            }
        }

        private static void MeasureLoadedDescendantCreate(Form form)
        {
            TabControl main = FindControl<TabControl>(form, "tbcMain");
            if (main == null)
                throw new InvalidOperationException("tbcMain was not found.");

            int originalMainIndex = main.SelectedIndex;
            for (int pageIndex = 1; pageIndex < main.TabPages.Count; pageIndex++)
            {
                main.SelectedIndex = originalMainIndex >= 0 && originalMainIndex < main.TabPages.Count
                    ? originalMainIndex
                    : 0;
                PumpMessages();
                MeasurePrecreateAndSelect(main, pageIndex, "loaded-main");
            }

            foreach (string nestedName in new[] { "tbcDrivers", "tbcBlades", "tbcItems", "tbcFlagData", "tbcUnknown" })
            {
                TabControl nested = FindControl<TabControl>(form, nestedName);
                if (nested == null)
                    continue;

                Control ownerControl = nested.Parent;
                while (ownerControl != null && !(ownerControl is TabPage))
                    ownerControl = ownerControl.Parent;

                if (ownerControl is TabPage owner && main.TabPages.Contains(owner))
                {
                    main.SelectedTab = owner;
                    PumpMessages();
                }

                int originalNestedIndex = nested.SelectedIndex;
                for (int pageIndex = 1; pageIndex < nested.TabPages.Count; pageIndex++)
                {
                    nested.SelectedIndex = originalNestedIndex >= 0 && originalNestedIndex < nested.TabPages.Count
                        ? originalNestedIndex
                        : 0;
                    PumpMessages();
                    MeasurePrecreateAndSelect(nested, pageIndex, "loaded-nested");
                }
            }

            if (originalMainIndex >= 0 && originalMainIndex < main.TabPages.Count)
                main.SelectedIndex = originalMainIndex;
            PumpMessages();
        }

        private static void MeasurePrecreateAndSelect(TabControl tabControl, int pageIndex, string scope)
        {
            TabPage page = tabControl.TabPages[pageIndex];
            List<Control> descendants = new List<Control>();
            CollectControls(page, descendants);

            ResetHandleProfile();
            Stopwatch createWatch = Stopwatch.StartNew();
            foreach (Control control in descendants)
                control.CreateControl();
            page.PerformLayout();
            createWatch.Stop();
            PumpMessages();
            Log(
                "PRECREATE scope=" + scope +
                " tab=" + tabControl.Name +
                " index=" + pageIndex +
                " page=" + page.Name +
                " controls=" + descendants.Count +
                " createMs=" + createWatch.Elapsed.TotalMilliseconds.ToString("F1") +
                HandleProfileText());

            ResetHandleProfile();
            Stopwatch selectWatch = Stopwatch.StartNew();
            tabControl.SelectedIndex = pageIndex;
            selectWatch.Stop();
            Stopwatch pumpWatch = Stopwatch.StartNew();
            PumpMessages();
            pumpWatch.Stop();
            Log(
                "PRECREATE_SELECT scope=" + scope +
                " tab=" + tabControl.Name +
                " index=" + pageIndex +
                " page=" + page.Name +
                " selectMs=" + selectWatch.Elapsed.TotalMilliseconds.ToString("F1") +
                " pumpMs=" + pumpWatch.Elapsed.TotalMilliseconds.ToString("F1") +
                HandleProfileText());
        }

        private static void MeasureLoadedOffscreenCreate(Form form)
        {
            TabControl main = FindControl<TabControl>(form, "tbcMain");
            if (main == null)
                throw new InvalidOperationException("tbcMain was not found.");

            int originalMainIndex = main.SelectedIndex;
            for (int pageIndex = 1; pageIndex < main.TabPages.Count; pageIndex++)
            {
                if (originalMainIndex >= 0 && originalMainIndex < main.TabPages.Count)
                    main.SelectedIndex = originalMainIndex;
                PumpMessages();
                MeasureOffscreenAndSelect(main, pageIndex, "loaded-main");
            }

            foreach (string nestedName in new[] { "tbcDrivers", "tbcBlades", "tbcItems", "tbcFlagData", "tbcUnknown" })
            {
                TabControl nested = FindControl<TabControl>(form, nestedName);
                if (nested == null)
                    continue;

                Control ownerControl = nested.Parent;
                while (ownerControl != null && !(ownerControl is TabPage))
                    ownerControl = ownerControl.Parent;

                if (ownerControl is TabPage owner && main.TabPages.Contains(owner))
                {
                    main.SelectedTab = owner;
                    PumpMessages();
                }

                int originalNestedIndex = nested.SelectedIndex;
                for (int pageIndex = 1; pageIndex < nested.TabPages.Count; pageIndex++)
                {
                    if (originalNestedIndex >= 0 && originalNestedIndex < nested.TabPages.Count)
                        nested.SelectedIndex = originalNestedIndex;
                    PumpMessages();
                    MeasureOffscreenAndSelect(nested, pageIndex, "loaded-nested");
                }
            }

            if (originalMainIndex >= 0 && originalMainIndex < main.TabPages.Count)
                main.SelectedIndex = originalMainIndex;
            PumpMessages();
        }

        private static void MeasureHiddenBeforeSelect(Form form)
        {
            TabControl main = FindControl<TabControl>(form, "tbcMain");
            if (main == null)
                throw new InvalidOperationException("tbcMain was not found.");

            int originalIndex = main.SelectedIndex;
            main.Selecting += (sender, eventArgs) =>
            {
                if (eventArgs.TabPage == null || eventArgs.TabPage.Name == "tabMain")
                    return;

                foreach (Control child in eventArgs.TabPage.Controls)
                    child.Visible = false;
            };

            foreach (int pageIndex in new[] { 1, 2, 6 })
            {
                if (originalIndex >= 0 && originalIndex < main.TabPages.Count)
                    main.SelectedIndex = originalIndex;
                PumpMessages();
                ResetHandleProfile();
                Stopwatch selectWatch = Stopwatch.StartNew();
                main.SelectedIndex = pageIndex;
                selectWatch.Stop();
                Stopwatch pumpWatch = Stopwatch.StartNew();
                PumpMessages();
                pumpWatch.Stop();
                Log(
                    "HIDDEN_SELECT page=" + main.TabPages[pageIndex].Name +
                    " selectMs=" + selectWatch.Elapsed.TotalMilliseconds.ToString("F1") +
                    " pumpMs=" + pumpWatch.Elapsed.TotalMilliseconds.ToString("F1") +
                    HandleProfileText());
            }
        }

        private static void MeasureHiddenCreateAfterSelect(Form form)
        {
            TabControl main = FindControl<TabControl>(form, "tbcMain");
            TabPage page = main.TabPages.Cast<TabPage>().First(candidate => candidate.Name == "tabDrivers");
            foreach (Control child in page.Controls)
                child.Visible = false;

            main.SelectedTab = page;
            PumpMessages();
            AttachHandleProfile(form);
            ResetHandleProfile();

            foreach (Control child in page.Controls)
            {
                Stopwatch watch = Stopwatch.StartNew();
                child.CreateControl();
                watch.Stop();
                Log(
                    "HIDDEN_CREATE_AFTER_SELECT control=" + child.Name +
                    " visible=" + child.Visible +
                    " ms=" + watch.Elapsed.TotalMilliseconds.ToString("F1") +
                    HandleProfileText());
                ResetHandleProfile();
            }
        }

        private static void MeasureProgressiveCreate(Form form)
        {
            TabControl main = FindControl<TabControl>(form, "tbcMain");
            TabPage page = main.TabPages.Cast<TabPage>().First(candidate => candidate.Name == "tabDrivers");
            foreach (Control child in page.Controls)
                SetVisibleRecursive(child, false);

            main.SelectedTab = page;
            PumpMessages();
            AttachHandleProfile(form);

            Queue<Control> queue = new Queue<Control>(page.Controls.Cast<Control>());
            int count = 0;
            while (queue.Count != 0)
            {
                Control control = queue.Dequeue();
                if (control is TabPage tabPage && tabPage.Parent is TabControl owner && owner.SelectedTab != tabPage)
                    continue;

                ResetHandleProfile();
                Stopwatch watch = Stopwatch.StartNew();
                control.Visible = true;
                control.CreateControl();
                watch.Stop();
                Log(
                    "PROGRESSIVE_CREATE index=" + count++ +
                    " control=" + control.Name +
                    " type=" + control.GetType().Name +
                    " ms=" + watch.Elapsed.TotalMilliseconds.ToString("F1") +
                    HandleProfileText());

                if (control is TabControl nested)
                {
                    if (nested.SelectedTab != null)
                        queue.Enqueue(nested.SelectedTab);
                    continue;
                }

                foreach (Control child in control.Controls)
                    queue.Enqueue(child);
            }
        }

        private static void SetVisibleRecursive(Control control, bool visible)
        {
            control.Visible = visible;
            foreach (Control child in control.Controls)
                SetVisibleRecursive(child, visible);
        }

        private static void ReplaceDataTableComboSources(Form form)
        {
            TabControl main = FindControl<TabControl>(form, "tbcMain");
            List<Control> controls = new List<Control>();
            CollectControls(main, controls);

            foreach (ComboBox combo in controls.OfType<ComboBox>())
            {
                DataTable table = combo.DataSource as DataTable;
                if (table == null || String.IsNullOrEmpty(combo.ValueMember) || String.IsNullOrEmpty(combo.DisplayMember))
                    continue;

                List<PerfComboItem> items = new List<PerfComboItem>(table.Rows.Count);
                foreach (DataRow row in table.Rows)
                    items.Add(new PerfComboItem(row[combo.ValueMember], Convert.ToString(row[combo.DisplayMember])));

                combo.DataSource = null;
                combo.DataSource = items;
                combo.DisplayMember = nameof(PerfComboItem.Text);
                combo.ValueMember = nameof(PerfComboItem.Value);
            }
        }

        private static void UnbindLargeComboSources(Form form)
        {
            TabControl main = FindControl<TabControl>(form, "tbcMain");
            List<Control> controls = new List<Control>();
            CollectControls(main, controls);

            foreach (ComboBox combo in controls.OfType<ComboBox>())
            {
                int count = combo.DataSource is DataTable table
                    ? table.Rows.Count
                    : combo.DataSource is System.Collections.ICollection collection
                        ? collection.Count
                        : 0;
                if (count <= 20)
                    continue;

                string textValue = combo.Text;
                combo.DataSource = null;
                combo.Items.Clear();
                combo.Text = textValue;
                Log("UNBOUND_COMBO name=" + combo.Name + " count=" + count);
            }
        }

        private static void MeasureUnboundComboTabs(Form form)
        {
            TabControl main = FindControl<TabControl>(form, "tbcMain");
            foreach (int pageIndex in new[] { 1, 2, 6 })
            {
                ResetHandleProfile();
                Stopwatch selectWatch = Stopwatch.StartNew();
                main.SelectedIndex = pageIndex;
                selectWatch.Stop();
                Stopwatch pumpWatch = Stopwatch.StartNew();
                PumpMessages();
                pumpWatch.Stop();
                Log(
                    "UNBOUND_SELECT page=" + main.TabPages[pageIndex].Name +
                    " selectMs=" + selectWatch.Elapsed.TotalMilliseconds.ToString("F1") +
                    " pumpMs=" + pumpWatch.Elapsed.TotalMilliseconds.ToString("F1") +
                    HandleProfileText());
            }
        }

        private static void MeasureDeferredComboSmoke(Form form)
        {
            ComboBox artCombo = FindControl<ComboBox>(form, "cbxDriverArtID");
            NumericUpDown artNumeric = FindControl<NumericUpDown>(form, "nudDriverArtID");
            Log("DEFERRED_SMOKE initialBound=" + (artCombo.DataSource != null) + " text=" + artCombo.Text);

            artNumeric.Value = Math.Min(1, artNumeric.Maximum);
            PumpMessages();
            Log("DEFERRED_SMOKE numericChangedBound=" + (artCombo.DataSource != null) + " text=" + artCombo.Text);

            InvokePrivate(artCombo, "OnDropDown", EventArgs.Empty);
            PumpMessages();
            Log("DEFERRED_SMOKE dropdownBound=" + (artCombo.DataSource != null) + " selectedValue=" + Convert.ToString(artCombo.SelectedValue));

            TabControl main = FindControl<TabControl>(form, "tbcMain");
            main.SelectedIndex = 1;
            PumpMessages();
            ListBox drivers = FindControl<ListBox>(form, "lbxDrivers");
            if (drivers.Items.Count > 1)
                drivers.SelectedIndex = 1;
            PumpMessages();
            Log("DEFERRED_SMOKE driverChanged index=" + drivers.SelectedIndex + " failures=0");
        }

        private static void MeasureOffscreenAndSelect(TabControl tabControl, int pageIndex, string scope)
        {
            TabPage page = tabControl.TabPages[pageIndex];

            ResetHandleProfile();
            Stopwatch warmupWatch = Stopwatch.StartNew();
            TabControlPerformance.WarmUpPagesOffscreen(
                tabControl,
                candidate => ReferenceEquals(candidate, page),
                false);
            warmupWatch.Stop();
            PumpMessages();
            Log(
                "OFFSCREEN_CREATE scope=" + scope +
                " tab=" + tabControl.Name +
                " index=" + pageIndex +
                " page=" + page.Name +
                " warmupMs=" + warmupWatch.Elapsed.TotalMilliseconds.ToString("F1") +
                HandleProfileText());

            ResetHandleProfile();
            Stopwatch selectWatch = Stopwatch.StartNew();
            tabControl.SelectedIndex = pageIndex;
            selectWatch.Stop();
            Stopwatch pumpWatch = Stopwatch.StartNew();
            PumpMessages();
            pumpWatch.Stop();
            Log(
                "OFFSCREEN_SELECT scope=" + scope +
                " tab=" + tabControl.Name +
                " index=" + pageIndex +
                " page=" + page.Name +
                " selectMs=" + selectWatch.Elapsed.TotalMilliseconds.ToString("F1") +
                " pumpMs=" + pumpWatch.Elapsed.TotalMilliseconds.ToString("F1") +
                HandleProfileText());
        }

        private static void CollectNestedTabControls(Control parent, List<TabControl> result)
        {
            foreach (Control child in parent.Controls)
            {
                if (child is TabControl tabControl)
                {
                    result.Add(tabControl);
                    continue;
                }

                CollectNestedTabControls(child, result);
            }
        }

        private static void DumpControlTree(Control control, int depth)
        {
            Log(new string(' ', depth * 2) + control.GetType().Name + " name=" + control.Name + " controls=" + control.Controls.Count);
            foreach (Control child in control.Controls)
                DumpControlTree(child, depth + 1);
        }

        private static void CollectControls(Control parent, List<Control> result)
        {
            foreach (Control child in parent.Controls)
            {
                result.Add(child);
                CollectControls(child, result);
            }
        }

        private static void AttachHandleProfile(Control root)
        {
            handleTypeCounts = new Dictionary<string, int>();
            handleCount = 0;
            AttachHandleProfileRecursive(root);
        }

        private static void AttachHandleProfileRecursive(Control control)
        {
            control.HandleCreated += HandleProfile_HandleCreated;
            foreach (Control child in control.Controls)
                AttachHandleProfileRecursive(child);
        }

        private static void HandleProfile_HandleCreated(object sender, EventArgs e)
        {
            handleCount++;
            string typeName = sender.GetType().Name;
            if (!handleTypeCounts.TryGetValue(typeName, out int count))
                count = 0;
            handleTypeCounts[typeName] = count + 1;
        }

        private static void ResetHandleProfile()
        {
            if (handleTypeCounts == null)
                return;

            handleCount = 0;
            handleTypeCounts.Clear();
        }

        private static string HandleProfileText()
        {
            if (handleTypeCounts == null)
                return string.Empty;

            string types = string.Join(",", handleTypeCounts
                .OrderByDescending(pair => pair.Value)
                .ThenBy(pair => pair.Key)
                .Take(8)
                .Select(pair => pair.Key + ":" + pair.Value));
            return " handles=" + handleCount + " types=" + types;
        }

        private static string offscreenMainTabTabName(TabControl tabControl, int index)
        {
            return index >= 0 && index < tabControl.TabPages.Count ? tabControl.TabPages[index].Name : String.Empty;
        }

        private static T FindControl<T>(Control parent, string name) where T : Control
        {
            foreach (Control child in parent.Controls)
            {
                if (child.Name == name && child is T match)
                    return match;

                T descendant = FindControl<T>(child, name);
                if (descendant != null)
                    return descendant;
            }

            return null;
        }

        private static void SetAndLogRows(Form form, string name, int rowCount)
        {
            DataGridView grid = FindControl<DataGridView>(form, name);
            Stopwatch watch = Stopwatch.StartNew();
            grid.RowCount = rowCount;
            watch.Stop();
            Log("FLAG_ROWS name=" + name + " ms=" + watch.Elapsed.TotalMilliseconds.ToString("F1"));
        }

        private static void InvokePrivate(object target, string methodName, params object[] arguments)
        {
            MethodInfo method = target.GetType().GetMethod(
                methodName,
                BindingFlags.Instance | BindingFlags.NonPublic);
            if (method == null)
                throw new MissingMethodException(target.GetType().FullName, methodName);

            try
            {
                method.Invoke(target, arguments);
            }
            catch (TargetInvocationException ex) when (ex.InnerException != null)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                throw;
            }
        }

        private sealed class PerfComboItem
        {
            public PerfComboItem(object value, string text)
            {
                Value = value;
                Text = text;
            }

            public object Value { get; }
            public string Text { get; }
        }
    }
}
