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
    public partial class FrmErrorBox : Form
    {
        public FrmErrorBox()
        {
            InitializeComponent();

            pbxError.Image = SystemIcons.Error.ToBitmap();
        }

        public FrmErrorBox(string message, string title, string boxContents) : this()
        {
            this.Text = title;
            lblErrorTitle.Text = title;
            txtErrorMsg.Text = message;

            txtErrorStacktrace.Text = boxContents;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
