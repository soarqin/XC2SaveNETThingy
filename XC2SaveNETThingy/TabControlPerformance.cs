using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace XC2SaveNETThingy
{
    /// <summary>
    /// Reduces flicker and forces hidden tab pages to create their child
    /// controls before the user first selects them.
    /// </summary>
    public static class TabControlPerformance
    {
        private const int WM_SETREDRAW = 0x000B;

        private static readonly PropertyInfo DoubleBufferedProperty =
            typeof(Control).GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);

        public static void Configure(Control root)
        {
            if (root == null)
                return;

            ConfigureControl(root);
        }

        public static void WarmUpAllPages(TabControl root)
        {
            if (root == null)
                return;

            int selectedIndex = root.SelectedIndex;
            root.SuspendLayout();

            try
            {
                root.CreateControl();
                SetRedraw(root, false);

                for (int index = 0; index < root.TabPages.Count; index++)
                {
                    root.SelectedIndex = index;
                    TabPage page = root.TabPages[index];
                    page.CreateControl();
                    page.PerformLayout();
                    WarmUpNestedTabControls(page);
                }
            }
            finally
            {
                if (selectedIndex >= 0 && selectedIndex < root.TabPages.Count)
                    root.SelectedIndex = selectedIndex;

                root.ResumeLayout(true);
                SetRedraw(root, true);
                root.Invalidate(true);
            }
        }

        /// <summary>
        /// Creates the native handles for selected pages while the pages are hosted
        /// by an off-screen tab control. This works for pages that are hidden by the
        /// real tab control, where CreateControl alone does not create descendants.
        /// </summary>
        public static void WarmUpPagesOffscreen(TabControl root, Func<TabPage, bool> shouldWarm, bool includeNested)
        {
            if (root == null || shouldWarm == null)
                return;

            int selectedIndex = root.SelectedIndex;
            root.SuspendLayout();

            using (Form host = new Form
            {
                ShowInTaskbar = false,
                FormBorderStyle = FormBorderStyle.None,
                ControlBox = false,
                StartPosition = FormStartPosition.Manual,
                Location = new System.Drawing.Point(-32000, -32000),
                Size = root.ClientSize.Width > 0 && root.ClientSize.Height > 0
                    ? root.ClientSize
                    : new System.Drawing.Size(1024, 768)
            })
            using (TabControl hostTabControl = new TabControl { Dock = DockStyle.Fill })
            {
                host.Controls.Add(hostTabControl);
                host.Show(root.FindForm());

                try
                {
                    List<TabPage> pages = new List<TabPage>();
                    foreach (TabPage page in root.TabPages)
                    {
                        if (shouldWarm(page))
                            pages.Add(page);
                    }

                    foreach (TabPage page in pages)
                    {
                        int pageIndex = root.TabPages.IndexOf(page);
                        if (pageIndex < 0)
                            continue;

                        root.TabPages.Remove(page);
                        hostTabControl.TabPages.Add(page);
                        hostTabControl.SelectedTab = page;
                        page.CreateControl();

                        if (includeNested)
                            WarmUpNestedTabControls(page);

                        hostTabControl.TabPages.Remove(page);
                        root.TabPages.Insert(Math.Min(pageIndex, root.TabPages.Count), page);
                    }
                }
                finally
                {
                    if (selectedIndex >= 0 && selectedIndex < root.TabPages.Count)
                        root.SelectedIndex = selectedIndex;

                    host.Hide();
                    root.ResumeLayout(true);
                }
            }
        }

        public static void WarmUpNestedPages(Control parent)
        {
            WarmUpNestedTabControls(parent);
        }

        public static void WarmUpPagesInPlace(TabControl root, Func<TabPage, bool> shouldWarm, bool includeNested)
        {
            if (root == null || shouldWarm == null)
                return;

            int selectedIndex = root.SelectedIndex;
            root.SuspendLayout();
            SetRedraw(root, false);

            try
            {
                List<TabPage> pages = new List<TabPage>();
                foreach (TabPage page in root.TabPages)
                {
                    if (shouldWarm(page))
                        pages.Add(page);
                }

                foreach (TabPage page in pages)
                {
                    root.SelectedTab = page;
                    page.CreateControl();
                    if (includeNested)
                        WarmUpNestedTabControls(page);
                }
            }
            finally
            {
                if (selectedIndex >= 0 && selectedIndex < root.TabPages.Count)
                    root.SelectedIndex = selectedIndex;

                root.ResumeLayout(true);
                SetRedraw(root, true);
                root.Invalidate(true);
            }
        }

        private static void ConfigureControl(Control control)
        {
            if (control is TabControl || control is DataGridView)
                SetDoubleBuffered(control);

            foreach (Control child in control.Controls)
                ConfigureControl(child);
        }

        private static void WarmUpNestedTabControls(Control parent)
        {
            foreach (Control child in parent.Controls)
            {
                if (child is TabControl nestedTabControl)
                    WarmUpAllPages(nestedTabControl);
                else
                    WarmUpNestedTabControls(child);
            }
        }

        private static void SetDoubleBuffered(Control control)
        {
            if (DoubleBufferedProperty == null || !DoubleBufferedProperty.CanWrite)
                return;

            try
            {
                DoubleBufferedProperty.SetValue(control, true, null);
            }
            catch (TargetInvocationException)
            {
                // A framework control may reject this implementation detail.
            }
        }

        private static bool SetRedraw(Control control, bool enabled)
        {
            if (!control.IsHandleCreated)
                return enabled;

            SendMessage(control.Handle, WM_SETREDRAW, enabled ? new IntPtr(1) : IntPtr.Zero, IntPtr.Zero);
            return enabled;
        }

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int message, IntPtr wParam, IntPtr lParam);
    }
}
