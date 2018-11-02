namespace XC2SaveNETThingy
{
    partial class frmFAQ
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmFAQ));
            this.gbxFAQ = new System.Windows.Forms.GroupBox();
            this.lblAnswer2 = new System.Windows.Forms.Label();
            this.lblQuestion2 = new System.Windows.Forms.Label();
            this.lblAnswer1 = new System.Windows.Forms.Label();
            this.lblQuestion1 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.gbxFAQ.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbxFAQ
            // 
            this.gbxFAQ.Controls.Add(this.lblAnswer2);
            this.gbxFAQ.Controls.Add(this.lblQuestion2);
            this.gbxFAQ.Controls.Add(this.lblAnswer1);
            this.gbxFAQ.Controls.Add(this.lblQuestion1);
            this.gbxFAQ.Location = new System.Drawing.Point(5, 3);
            this.gbxFAQ.Name = "gbxFAQ";
            this.gbxFAQ.Size = new System.Drawing.Size(581, 158);
            this.gbxFAQ.TabIndex = 0;
            this.gbxFAQ.TabStop = false;
            this.gbxFAQ.Text = "FAQ";
            // 
            // lblAnswer2
            // 
            this.lblAnswer2.AutoSize = true;
            this.lblAnswer2.Location = new System.Drawing.Point(6, 126);
            this.lblAnswer2.Name = "lblAnswer2";
            this.lblAnswer2.Size = new System.Drawing.Size(557, 26);
            this.lblAnswer2.TabIndex = 3;
            this.lblAnswer2.Text = resources.GetString("lblAnswer2.Text");
            // 
            // lblQuestion2
            // 
            this.lblQuestion2.AutoSize = true;
            this.lblQuestion2.Location = new System.Drawing.Point(6, 103);
            this.lblQuestion2.Name = "lblQuestion2";
            this.lblQuestion2.Size = new System.Drawing.Size(338, 13);
            this.lblQuestion2.TabIndex = 2;
            this.lblQuestion2.Text = "Q: Can this edit fields to values that cannot be done normally in-game?";
            // 
            // lblAnswer1
            // 
            this.lblAnswer1.AutoSize = true;
            this.lblAnswer1.Location = new System.Drawing.Point(6, 42);
            this.lblAnswer1.Name = "lblAnswer1";
            this.lblAnswer1.Size = new System.Drawing.Size(573, 39);
            this.lblAnswer1.TabIndex = 1;
            this.lblAnswer1.Text = resources.GetString("lblAnswer1.Text");
            // 
            // lblQuestion1
            // 
            this.lblQuestion1.AutoSize = true;
            this.lblQuestion1.Location = new System.Drawing.Point(6, 16);
            this.lblQuestion1.Name = "lblQuestion1";
            this.lblQuestion1.Size = new System.Drawing.Size(262, 13);
            this.lblQuestion1.TabIndex = 0;
            this.lblQuestion1.Text = "Q: Why does some stuff I\'ve edited not seem to save?";
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(264, 167);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // frmFAQ
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(594, 202);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.gbxFAQ);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmFAQ";
            this.Text = "FAQ";
            this.gbxFAQ.ResumeLayout(false);
            this.gbxFAQ.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbxFAQ;
        private System.Windows.Forms.Label lblAnswer2;
        private System.Windows.Forms.Label lblQuestion2;
        private System.Windows.Forms.Label lblAnswer1;
        private System.Windows.Forms.Label lblQuestion1;
        private System.Windows.Forms.Button btnOK;
    }
}