using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XC2SaveNETThingy
{
    public partial class frmFAQ : Form
    {
        private Dictionary<string, string> FrmFAQText;

        public frmFAQ() => InitializeComponent();
        public frmFAQ(XC2Data.LANG l) : this()
        {
            FrmFAQText = XC2Data.GetFrmFAQText(l);
            UpdateControlText(this);
        }
        private void btnOK_Click(object sender, EventArgs e) => Close();

        void UpdateControlText(Control ctl)
        {
            if (FrmFAQText.ContainsKey(ctl.Name))
                ctl.Text = FrmFAQText[ctl.Name].Replace(@"\n", Environment.NewLine);

            foreach (Control chctl in ctl.Controls)
                UpdateControlText(chctl);
        }
    }
}
