namespace XC2SaveNETThingy
{
    partial class ItemEditPanel
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.nudItemID = new System.Windows.Forms.NumericUpDown();
            this.cbxItemID = new System.Windows.Forms.ComboBox();
            this.nudItemType = new System.Windows.Forms.NumericUpDown();
            this.cbxItemType = new System.Windows.Forms.ComboBox();
            this.nudItemQty = new System.Windows.Forms.NumericUpDown();
            this.chkItemEquipped = new System.Windows.Forms.CheckBox();
            this.nudItemTimeHours = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.nudItemTimeMinutes = new System.Windows.Forms.NumericUpDown();
            this.nudItemTimeSeconds = new System.Windows.Forms.NumericUpDown();
            this.nudItemSerial = new System.Windows.Forms.NumericUpDown();
            this.chkItemUnk1 = new System.Windows.Forms.CheckBox();
            this.chkItemUnk2 = new System.Windows.Forms.CheckBox();
            this.nudItenUnk3 = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.nudItemID)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudItemType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudItemQty)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudItemTimeHours)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudItemTimeMinutes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudItemTimeSeconds)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudItemSerial)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudItenUnk3)).BeginInit();
            this.SuspendLayout();
            // 
            // nudItemID
            // 
            this.nudItemID.Location = new System.Drawing.Point(0, 0);
            this.nudItemID.Maximum = new decimal(new int[] {
            8191,
            0,
            0,
            0});
            this.nudItemID.Name = "nudItemID";
            this.nudItemID.Size = new System.Drawing.Size(59, 20);
            this.nudItemID.TabIndex = 0;
            // 
            // cbxItemID
            // 
            this.cbxItemID.FormattingEnabled = true;
            this.cbxItemID.Location = new System.Drawing.Point(58, 0);
            this.cbxItemID.Name = "cbxItemID";
            this.cbxItemID.Size = new System.Drawing.Size(121, 21);
            this.cbxItemID.TabIndex = 1;
            // 
            // nudItemType
            // 
            this.nudItemType.Location = new System.Drawing.Point(185, 0);
            this.nudItemType.Maximum = new decimal(new int[] {
            63,
            0,
            0,
            0});
            this.nudItemType.Name = "nudItemType";
            this.nudItemType.Size = new System.Drawing.Size(39, 20);
            this.nudItemType.TabIndex = 2;
            // 
            // cbxItemType
            // 
            this.cbxItemType.FormattingEnabled = true;
            this.cbxItemType.Location = new System.Drawing.Point(223, 0);
            this.cbxItemType.Name = "cbxItemType";
            this.cbxItemType.Size = new System.Drawing.Size(121, 21);
            this.cbxItemType.TabIndex = 3;
            // 
            // nudItemQty
            // 
            this.nudItemQty.Location = new System.Drawing.Point(350, 0);
            this.nudItemQty.Maximum = new decimal(new int[] {
            1023,
            0,
            0,
            0});
            this.nudItemQty.Name = "nudItemQty";
            this.nudItemQty.Size = new System.Drawing.Size(51, 20);
            this.nudItemQty.TabIndex = 4;
            // 
            // chkItemEquipped
            // 
            this.chkItemEquipped.AutoSize = true;
            this.chkItemEquipped.Location = new System.Drawing.Point(407, 2);
            this.chkItemEquipped.Name = "chkItemEquipped";
            this.chkItemEquipped.Size = new System.Drawing.Size(71, 17);
            this.chkItemEquipped.TabIndex = 5;
            this.chkItemEquipped.Text = "Equipped";
            this.chkItemEquipped.UseVisualStyleBackColor = true;
            // 
            // nudItemTimeHours
            // 
            this.nudItemTimeHours.Location = new System.Drawing.Point(484, 0);
            this.nudItemTimeHours.Maximum = new decimal(new int[] {
            1048575,
            0,
            0,
            0});
            this.nudItemTimeHours.Name = "nudItemTimeHours";
            this.nudItemTimeHours.Size = new System.Drawing.Size(59, 20);
            this.nudItemTimeHours.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(543, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(10, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = ":";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(591, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(10, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = ":";
            // 
            // nudItemTimeMinutes
            // 
            this.nudItemTimeMinutes.Location = new System.Drawing.Point(552, 0);
            this.nudItemTimeMinutes.Maximum = new decimal(new int[] {
            59,
            0,
            0,
            0});
            this.nudItemTimeMinutes.Name = "nudItemTimeMinutes";
            this.nudItemTimeMinutes.Size = new System.Drawing.Size(39, 20);
            this.nudItemTimeMinutes.TabIndex = 8;
            // 
            // nudItemTimeSeconds
            // 
            this.nudItemTimeSeconds.Location = new System.Drawing.Point(600, 0);
            this.nudItemTimeSeconds.Maximum = new decimal(new int[] {
            59,
            0,
            0,
            0});
            this.nudItemTimeSeconds.Name = "nudItemTimeSeconds";
            this.nudItemTimeSeconds.Size = new System.Drawing.Size(39, 20);
            this.nudItemTimeSeconds.TabIndex = 10;
            // 
            // nudItemSerial
            // 
            this.nudItemSerial.Location = new System.Drawing.Point(655, 0);
            this.nudItemSerial.Maximum = new decimal(new int[] {
            67108863,
            0,
            0,
            0});
            this.nudItemSerial.Name = "nudItemSerial";
            this.nudItemSerial.Size = new System.Drawing.Size(77, 20);
            this.nudItemSerial.TabIndex = 11;
            // 
            // chkItemUnk1
            // 
            this.chkItemUnk1.AutoSize = true;
            this.chkItemUnk1.Location = new System.Drawing.Point(738, 2);
            this.chkItemUnk1.Name = "chkItemUnk1";
            this.chkItemUnk1.Size = new System.Drawing.Size(52, 17);
            this.chkItemUnk1.TabIndex = 12;
            this.chkItemUnk1.Text = "Unk1";
            this.chkItemUnk1.UseVisualStyleBackColor = true;
            // 
            // chkItemUnk2
            // 
            this.chkItemUnk2.AutoSize = true;
            this.chkItemUnk2.Location = new System.Drawing.Point(796, 2);
            this.chkItemUnk2.Name = "chkItemUnk2";
            this.chkItemUnk2.Size = new System.Drawing.Size(52, 17);
            this.chkItemUnk2.TabIndex = 13;
            this.chkItemUnk2.Text = "Unk2";
            this.chkItemUnk2.UseVisualStyleBackColor = true;
            // 
            // nudItenUnk3
            // 
            this.nudItenUnk3.Location = new System.Drawing.Point(854, 0);
            this.nudItenUnk3.Maximum = new decimal(new int[] {
            63,
            0,
            0,
            0});
            this.nudItenUnk3.Name = "nudItenUnk3";
            this.nudItenUnk3.Size = new System.Drawing.Size(51, 20);
            this.nudItenUnk3.TabIndex = 14;
            // 
            // ItemEditPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.nudItenUnk3);
            this.Controls.Add(this.chkItemUnk2);
            this.Controls.Add(this.chkItemUnk1);
            this.Controls.Add(this.nudItemSerial);
            this.Controls.Add(this.nudItemTimeSeconds);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.nudItemTimeMinutes);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.nudItemTimeHours);
            this.Controls.Add(this.chkItemEquipped);
            this.Controls.Add(this.nudItemQty);
            this.Controls.Add(this.cbxItemType);
            this.Controls.Add(this.nudItemType);
            this.Controls.Add(this.cbxItemID);
            this.Controls.Add(this.nudItemID);
            this.Name = "ItemEditPanel";
            this.Size = new System.Drawing.Size(978, 21);
            ((System.ComponentModel.ISupportInitialize)(this.nudItemID)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudItemType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudItemQty)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudItemTimeHours)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudItemTimeMinutes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudItemTimeSeconds)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudItemSerial)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudItenUnk3)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown nudItemID;
        private System.Windows.Forms.ComboBox cbxItemID;
        private System.Windows.Forms.NumericUpDown nudItemType;
        private System.Windows.Forms.ComboBox cbxItemType;
        private System.Windows.Forms.NumericUpDown nudItemQty;
        private System.Windows.Forms.CheckBox chkItemEquipped;
        private System.Windows.Forms.NumericUpDown nudItemTimeHours;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown nudItemTimeMinutes;
        private System.Windows.Forms.NumericUpDown nudItemTimeSeconds;
        private System.Windows.Forms.NumericUpDown nudItemSerial;
        private System.Windows.Forms.CheckBox chkItemUnk1;
        private System.Windows.Forms.CheckBox chkItemUnk2;
        private System.Windows.Forms.NumericUpDown nudItenUnk3;
    }
}
