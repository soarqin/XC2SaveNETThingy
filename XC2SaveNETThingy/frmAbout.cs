using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XC2SaveNETThingy
{
    public partial class frmAbout : Form
    {
        private Dictionary<string, string> FrmAboutText;

        public frmAbout(string appDesc, Icon icon, XC2Data.LANG l)
        {
            InitializeComponent();

            this.Icon = icon;
            lblDesc.Text = appDesc;
            FrmAboutText = XC2Data.GetFrmAboutText(LanguageCatalog.ToCurrentDataLanguage(Localization.CurrentLanguageCode));

            UpdateControlText(this);
        }
        private void frmAbout_Load(object sender, EventArgs e) => lblAbout.Text = Application.ProductName + " ver" + Application.ProductVersion + " by " + Application.CompanyName;
        private void btnOK_Click(object sender, EventArgs e) => Close();
        private void linkLabel_Click(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LinkLabel ll = (LinkLabel)sender;

            ll.LinkVisited = true;
            Process.Start(ll.Text);
        }

        void UpdateControlText(Control ctl)
        {
            if (FrmAboutText.ContainsKey(ctl.Name))
                ctl.Text = FrmAboutText[ctl.Name].Replace(@"\n", Environment.NewLine);

            foreach (Control chctl in ctl.Controls)
                UpdateControlText(chctl);
        }
    }
}
