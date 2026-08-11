using System.Collections.Generic;
using System.Windows.Forms;

namespace XC2SaveNETThingy
{
    public static class DeferredTabPageState
    {
        public static void MarkVisiblePathReady(Control parent, ISet<TabPage> deferredPages)
        {
            if (parent == null || deferredPages == null)
                return;

            if (parent is TabControl tabControl)
            {
                if (tabControl.SelectedTab != null)
                    MarkVisiblePathReady(tabControl.SelectedTab, deferredPages);
                return;
            }

            if (parent is TabPage page)
                deferredPages.Remove(page);

            foreach (Control child in parent.Controls)
                MarkVisiblePathReady(child, deferredPages);
        }
    }
}
