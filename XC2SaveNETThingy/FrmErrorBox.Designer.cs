namespace XC2SaveNETThingy
{
    partial class FrmErrorBox
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
            this.lblErrorTitle = new System.Windows.Forms.Label();
            this.txtErrorStacktrace = new System.Windows.Forms.TextBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.lblStacktrace = new System.Windows.Forms.Label();
            this.pbxError = new System.Windows.Forms.PictureBox();
            this.lblError = new System.Windows.Forms.Label();
            this.txtErrorMsg = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbxError)).BeginInit();
            this.SuspendLayout();
            // 
            // lblErrorTitle
            // 
            this.lblErrorTitle.AutoSize = true;
            this.lblErrorTitle.Location = new System.Drawing.Point(118, 12);
            this.lblErrorTitle.Name = "lblErrorTitle";
            this.lblErrorTitle.Size = new System.Drawing.Size(35, 13);
            this.lblErrorTitle.TabIndex = 0;
            this.lblErrorTitle.Text = "label1";
            // 
            // txtErrorStacktrace
            // 
            this.txtErrorStacktrace.Location = new System.Drawing.Point(12, 227);
            this.txtErrorStacktrace.Multiline = true;
            this.txtErrorStacktrace.Name = "txtErrorStacktrace";
            this.txtErrorStacktrace.ReadOnly = true;
            this.txtErrorStacktrace.Size = new System.Drawing.Size(470, 179);
            this.txtErrorStacktrace.TabIndex = 1;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(210, 424);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // lblStacktrace
            // 
            this.lblStacktrace.AutoSize = true;
            this.lblStacktrace.Location = new System.Drawing.Point(12, 211);
            this.lblStacktrace.Name = "lblStacktrace";
            this.lblStacktrace.Size = new System.Drawing.Size(129, 13);
            this.lblStacktrace.TabIndex = 3;
            this.lblStacktrace.Text = "Stacktrace for Developer:";
            // 
            // pbxError
            // 
            this.pbxError.Location = new System.Drawing.Point(12, 12);
            this.pbxError.Name = "pbxError";
            this.pbxError.Size = new System.Drawing.Size(100, 50);
            this.pbxError.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pbxError.TabIndex = 4;
            this.pbxError.TabStop = false;
            // 
            // lblError
            // 
            this.lblError.AutoSize = true;
            this.lblError.Location = new System.Drawing.Point(118, 34);
            this.lblError.Name = "lblError";
            this.lblError.Size = new System.Drawing.Size(78, 13);
            this.lblError.TabIndex = 5;
            this.lblError.Text = "Error Message:";
            // 
            // txtErrorMsg
            // 
            this.txtErrorMsg.Location = new System.Drawing.Point(121, 50);
            this.txtErrorMsg.Multiline = true;
            this.txtErrorMsg.Name = "txtErrorMsg";
            this.txtErrorMsg.ReadOnly = true;
            this.txtErrorMsg.Size = new System.Drawing.Size(361, 158);
            this.txtErrorMsg.TabIndex = 6;
            // 
            // FrmErrorBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(494, 459);
            this.Controls.Add(this.txtErrorMsg);
            this.Controls.Add(this.lblError);
            this.Controls.Add(this.pbxError);
            this.Controls.Add(this.lblStacktrace);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.txtErrorStacktrace);
            this.Controls.Add(this.lblErrorTitle);
            this.Name = "FrmErrorBox";
            this.Text = "FrmErrorBox";
            ((System.ComponentModel.ISupportInitialize)(this.pbxError)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblErrorTitle;
        private System.Windows.Forms.TextBox txtErrorStacktrace;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label lblStacktrace;
        private System.Windows.Forms.PictureBox pbxError;
        private System.Windows.Forms.Label lblError;
        private System.Windows.Forms.TextBox txtErrorMsg;
    }
}