using DataGridViewNumericUpDownElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XC2SaveNETThingy
{
    public static class DataGridViewColumnFactory
    {
        public enum DgvColumnType
        {
            TextBox,
            NumericUpDown,
            ComboBox,
            CheckBox
        }

        public static DataGridViewColumn CreateDgvColumn(DgvColumnType columnType, string dataPropertyName, string headerText, string dataType = null, object dataSource = null, int minimum = 0, int maximum = 100)
        {
            switch (columnType)
            {
                case DgvColumnType.TextBox:
                    DataGridViewTextBoxColumn cTxt = new DataGridViewTextBoxColumn();
                    cTxt.DataPropertyName = dataPropertyName;
                    cTxt.HeaderText = headerText;
                    return cTxt;

                case DgvColumnType.NumericUpDown:
                    DataGridViewNumericUpDownColumn cNud = new DataGridViewNumericUpDownColumn();
                    cNud.DataPropertyName = dataPropertyName;
                    cNud.HeaderText = headerText;
                    if (dataType != null)
                        cNud.ValueType = Type.GetType(dataType);
                    cNud.Maximum = maximum;
                    cNud.Minimum = minimum;
                    return cNud;

                case DgvColumnType.ComboBox:
                    DataGridViewComboBoxColumn cCbx = new DataGridViewComboBoxColumn();
                    //cCbx.DataPropertyName = dataPropertyName;
                    cCbx.HeaderText = headerText;
                    cCbx.DisplayMember = "Value";
                    cCbx.ValueMember = "Key";
                    cCbx.DataSource = dataSource;
                    return cCbx;

                case DgvColumnType.CheckBox:
                    DataGridViewCheckBoxColumn cChk = new DataGridViewCheckBoxColumn();
                    cChk.DataPropertyName = dataPropertyName;
                    cChk.HeaderText = headerText;
                    return cChk;
            }

            string message = "Invalid Column Type was specified." + Environment.NewLine +
                Environment.NewLine +
                "Valid Column Types:" + Environment.NewLine +
                "0: TextBox Column" + Environment.NewLine +
                "1: NumericUpDown Column" + Environment.NewLine +
                "2: ComboBox Column" + Environment.NewLine +
                "3: CheckBox Column" + Environment.NewLine +
                 Environment.NewLine +
                 "Requested Column Type: " + columnType;

            throw new Exception(message);
        }

        public static DataGridViewColumn CreateDgvColumn_ALT(DgvColumnType columnType, string dataPropertyName, string headerText, string dataType = null, object dataSource = null, int minimum = 0, int maximum = 100)
        {
            switch (columnType)
            {
                case DgvColumnType.TextBox:
                    DataGridViewTextBoxColumn cTxt = new DataGridViewTextBoxColumn();
                    cTxt.DataPropertyName = dataPropertyName;
                    cTxt.HeaderText = headerText;
                    return cTxt;

                case DgvColumnType.NumericUpDown:
                    DataGridViewNumericUpDownColumn cNud = new DataGridViewNumericUpDownColumn();
                    cNud.DataPropertyName = dataPropertyName;
                    cNud.HeaderText = headerText;
                    if (dataType != null)
                        cNud.ValueType = Type.GetType(dataType);
                    cNud.Maximum = maximum;
                    cNud.Minimum = minimum;
                    return cNud;

                case DgvColumnType.ComboBox:
                    DataGridViewComboBoxColumn cCbx = new DataGridViewComboBoxColumn();
                    //cCbx.DataPropertyName = dataPropertyName;
                    cCbx.HeaderText = headerText;
                    cCbx.DisplayMember = "Name";
                    cCbx.ValueMember = "ID";
                    cCbx.DataSource = dataSource;
                    return cCbx;

                case DgvColumnType.CheckBox:
                    DataGridViewCheckBoxColumn cChk = new DataGridViewCheckBoxColumn();
                    cChk.DataPropertyName = dataPropertyName;
                    cChk.HeaderText = headerText;
                    return cChk;
            }

            string message = "Invalid Column Type was specified." + Environment.NewLine +
                Environment.NewLine +
                "Valid Column Types:" + Environment.NewLine +
                "0: TextBox Column" + Environment.NewLine +
                "1: NumericUpDown Column" + Environment.NewLine +
                "2: ComboBox Column" + Environment.NewLine +
                "3: CheckBox Column" + Environment.NewLine +
                 Environment.NewLine +
                 "Requested Column Type: " + columnType;

            throw new Exception(message);
        }
    }
}
