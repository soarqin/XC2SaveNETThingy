using DataGridViewNumericUpDownElements;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XC2SaveNETThingy
{
    public static class DataGridViewColumnFactory
    {
        private const string ReservedLookupPropertyName = "XC2SaveNETThingy.ReservedLookup";

        public enum DgvColumnType
        {
            TextBox,
            NumericUpDown,
            ComboBox,
            CheckBox
        }

        /// <summary>
        /// Applies a prominent warning style to an item row marked as Reserved in
        /// the lookup table. The Reserved column is optional for compatibility with
        /// older data files, so rows remain unchanged when it is not available.
        /// </summary>
        public static void ApplyReservedItemStyle(DataGridView dgv, int rowIndex)
        {
            if (dgv == null || rowIndex < 0 || rowIndex >= dgv.Rows.Count)
                return;

            DataGridViewRow row = dgv.Rows[rowIndex];
            bool reserved = TryGetReservedItem(dgv, rowIndex);

            if (!reserved && !row.HasDefaultCellStyle && row.Cells[1].ToolTipText == null)
                return;

            if (reserved)
            {
                row.DefaultCellStyle.BackColor = Color.MistyRose;
                row.DefaultCellStyle.ForeColor = Color.Maroon;
                row.DefaultCellStyle.SelectionBackColor = Color.IndianRed;
                row.DefaultCellStyle.SelectionForeColor = Color.White;
                row.Cells[1].ToolTipText = "Reserved item: retained/invalid game data; edit with care.";
            }
            else
            {
                // Empty style values restore inheritance (including alternating rows).
                row.DefaultCellStyle.BackColor = Color.Empty;
                row.DefaultCellStyle.ForeColor = Color.Empty;
                row.DefaultCellStyle.SelectionBackColor = Color.Empty;
                row.DefaultCellStyle.SelectionForeColor = Color.Empty;
                row.Cells[1].ToolTipText = null;
            }
        }

        /// <summary>
        /// Applies the initial warning style only to rows that are actually
        /// marked as reserved. Ordinary rows remain shared and are not touched
        /// during the initial bind.
        /// </summary>
        public static void InitializeReservedItemStyles(DataGridView dgv, DataTable itemTable)
        {
            if (dgv == null || itemTable == null)
                return;

            Dictionary<UInt32, bool> reservedLookup = GetReservedLookup(itemTable);
            if (reservedLookup == null)
                return;

            for (int rowIndex = 0; rowIndex < dgv.Rows.Count; rowIndex++)
            {
                DataGridViewRow row = dgv.Rows[rowIndex];
                if (row.IsNewRow || !UInt32.TryParse(Convert.ToString(row.Cells[0].Value), out UInt32 id))
                    continue;

                if (reservedLookup.TryGetValue(id, out bool reserved) && reserved)
                    ApplyReservedItemStyle(dgv, rowIndex);
            }
        }

        public static bool IsReservedItem(DataTable itemTable, UInt32 id)
        {
            Dictionary<UInt32, bool> reservedLookup = GetReservedLookup(itemTable);
            return reservedLookup != null &&
                reservedLookup.TryGetValue(id, out bool reserved) && reserved;
        }

        private static bool TryGetReservedItem(DataGridView dgv, int rowIndex)
        {
            if (dgv.Columns.Count < 2 || dgv.Rows[rowIndex].Cells.Count < 2)
                return false;

            object idValue = dgv.Rows[rowIndex].Cells[0].Value;
            if (idValue == null || idValue == DBNull.Value || !UInt32.TryParse(idValue.ToString(), out UInt32 id))
                return false;

            DataGridViewComboBoxColumn nameColumn = dgv.Columns[1] as DataGridViewComboBoxColumn;
            DataTable itemTable = nameColumn?.DataSource as DataTable;
            if (itemTable == null && dgv.Tag is ItemGridBinding itemGridBinding)
                itemTable = itemGridBinding.ItemNames;
            Dictionary<UInt32, bool> reservedLookup = GetReservedLookup(itemTable);
            if (reservedLookup == null)
                return false;

            return reservedLookup.TryGetValue(id, out bool reserved) && reserved;
        }

        private static Dictionary<UInt32, bool> GetReservedLookup(DataTable table)
        {
            if (table == null || !table.Columns.Contains("ID") || !table.Columns.Contains("Reserved"))
                return null;

            Dictionary<UInt32, bool> lookup = table.ExtendedProperties[ReservedLookupPropertyName] as Dictionary<UInt32, bool>;
            if (lookup != null)
                return lookup;

            lookup = new Dictionary<UInt32, bool>();
            foreach (DataRow row in table.Rows)
            {
                if (!UInt32.TryParse(Convert.ToString(row["ID"]), out UInt32 id))
                    continue;

                object reservedValue = row["Reserved"];
                bool reserved = false;
                if (reservedValue != DBNull.Value && reservedValue != null)
                {
                    if (reservedValue is bool boolValue)
                        reserved = boolValue;
                    else if (Int64.TryParse(Convert.ToString(reservedValue), out Int64 integerValue))
                        reserved = integerValue != 0;
                    else
                        reserved = string.Equals(Convert.ToString(reservedValue), "true", StringComparison.OrdinalIgnoreCase);
                }

                lookup[id] = reserved;
            }

            table.ExtendedProperties[ReservedLookupPropertyName] = lookup;
            return lookup;
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
                col.DataPropertyName = dataPropertyName;
                if (dataType != null && col is DataGridViewComboBoxColumn)
                    col.ValueType = Type.GetType(dataType);
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
