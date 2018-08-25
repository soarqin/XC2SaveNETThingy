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

        public static DataGridViewColumn CreateDgvColumn(DgvColumnType columnType, string dataPropertyName, string headerText, string dataType = null, object dataSource = null, int width = 0, int minimum = 0, UInt32 maximum = 100)
        {
            DataGridViewColumn col = null;

            switch (columnType)
            {
                case DgvColumnType.TextBox:
                    col = new DataGridViewTextBoxColumn();
                    break;

                case DgvColumnType.NumericUpDown:
                    col = new DataGridViewNumericUpDownColumn();
                    if (dataType != null)
                        col.ValueType = Type.GetType(dataType);
                    ((DataGridViewNumericUpDownColumn)col).Maximum = maximum;
                    ((DataGridViewNumericUpDownColumn)col).Minimum = minimum;
                    break;

                case DgvColumnType.ComboBox:
                    col = new DataGridViewComboBoxColumn();
                    ((DataGridViewComboBoxColumn)col).DisplayMember = "Name";
                    ((DataGridViewComboBoxColumn)col).ValueMember = "ID";
                    ((DataGridViewComboBoxColumn)col).DataSource = dataSource;
                    break;

                case DgvColumnType.CheckBox:
                    col = new DataGridViewCheckBoxColumn();
                    break;
            }
            if (col != null)
            {
                if (!(col.GetType() == typeof(DataGridViewComboBoxColumn)))
                    col.DataPropertyName = dataPropertyName;
                col.HeaderText = headerText;
                if (width != 0)
                    col.Width = width;
                col.Resizable = DataGridViewTriState.False;

                return col;
            }
            else
            {
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
}
