using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Windows.Forms;

namespace XC2SaveNETThingy
{
    /// <summary>
    /// Keeps large ComboBox data sources out of the first native handle creation.
    /// The current value remains visible; the full source is attached when the
    /// user opens the combo box.
    /// </summary>
    public sealed class DeferredComboBindingManager : IDisposable
    {
        private readonly Dictionary<ComboBox, BindingState> bindings =
            new Dictionary<ComboBox, BindingState>();

        private sealed class BindingState
        {
            public object DataSource;
            public string DisplayMember;
            public string ValueMember;
            public object SelectedItem;
            public object SelectedValue;
            public string DisplayText;
            public NumericUpDown NumericControl;
            public bool IsBound;
        }

        public int Count => bindings.Count;

        public void DeferLargeBindings(Control root, int minimumItemCount, Func<ComboBox, bool> shouldSkip)
        {
            if (root == null)
                return;

            List<Control> controls = new List<Control>();
            CollectControls(root, controls);

            foreach (ComboBox combo in controls.OfTypeComboBoxes())
            {
                if (combo.DataSource == null || bindings.ContainsKey(combo) ||
                    (shouldSkip != null && shouldSkip(combo)))
                    continue;

                int itemCount = GetItemCount(combo.DataSource);
                if (itemCount <= minimumItemCount)
                    continue;

                Defer(combo, controls);
            }
        }

        public void EnsureBound(Control root)
        {
            if (root == null)
                return;

            List<ComboBox> combos = new List<ComboBox>();
            foreach (KeyValuePair<ComboBox, BindingState> pair in bindings)
            {
                if (IsDescendantOrSelf(pair.Key, root))
                    combos.Add(pair.Key);
            }

            foreach (ComboBox combo in combos)
                Bind(combo);
        }

        public void Dispose()
        {
            foreach (KeyValuePair<ComboBox, BindingState> pair in bindings)
            {
                pair.Key.DropDown -= DeferredCombo_DropDown;
                if (pair.Value.NumericControl != null)
                    pair.Value.NumericControl.ValueChanged -= DeferredNumeric_ValueChanged;
            }

            bindings.Clear();
        }

        private void Defer(ComboBox combo, List<Control> controls)
        {
            BindingState state = new BindingState
            {
                DataSource = combo.DataSource,
                DisplayMember = combo.DisplayMember,
                ValueMember = combo.ValueMember,
                SelectedItem = combo.SelectedItem,
                SelectedValue = combo.SelectedValue,
                DisplayText = combo.Text,
                NumericControl = FindNumericCounterpart(combo, controls)
            };

            bindings.Add(combo, state);
            combo.DataSource = null;
            combo.Items.Clear();
            combo.Text = state.DisplayText;
            combo.DropDown += DeferredCombo_DropDown;

            if (state.NumericControl != null)
            {
                state.NumericControl.ValueChanged += DeferredNumeric_ValueChanged;
                UpdateDeferredDisplay(combo, state);
            }
        }

        private void DeferredCombo_DropDown(object sender, EventArgs e)
        {
            Bind((ComboBox)sender);
        }

        private void DeferredNumeric_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown numeric = (NumericUpDown)sender;
            foreach (KeyValuePair<ComboBox, BindingState> pair in bindings)
            {
                if (ReferenceEquals(pair.Value.NumericControl, numeric))
                    UpdateDeferredDisplay(pair.Key, pair.Value);
            }
        }

        private void Bind(ComboBox combo)
        {
            if (!bindings.TryGetValue(combo, out BindingState state) || state.IsBound)
                return;

            object desiredValue = GetDesiredValue(state);
            object desiredItem = FindItem(state, desiredValue);

            combo.DataSource = state.DataSource;
            combo.DisplayMember = state.DisplayMember;
            combo.ValueMember = state.ValueMember;

            if (!String.IsNullOrEmpty(state.ValueMember))
                combo.SelectedValue = desiredValue ?? state.SelectedValue;
            else if (desiredItem != null)
                combo.SelectedItem = desiredItem;
            else
                combo.SelectedItem = state.SelectedItem;

            state.IsBound = true;
            combo.DropDown -= DeferredCombo_DropDown;
            if (state.NumericControl != null)
                state.NumericControl.ValueChanged -= DeferredNumeric_ValueChanged;
        }

        private static void UpdateDeferredDisplay(ComboBox combo, BindingState state)
        {
            if (state.IsBound)
                return;

            object desiredValue = GetDesiredValue(state);
            object item = FindItem(state, desiredValue);
            if (item != null)
            {
                state.SelectedItem = item;
                state.SelectedValue = desiredValue;
                state.DisplayText = GetDisplayText(item, state.DisplayMember);
            }

            combo.Text = state.DisplayText ?? String.Empty;
        }

        private static object GetDesiredValue(BindingState state)
        {
            if (state.NumericControl != null)
                return state.NumericControl.Value;

            return String.IsNullOrEmpty(state.ValueMember)
                ? state.SelectedItem
                : state.SelectedValue;
        }

        private static object FindItem(BindingState state, object desiredValue)
        {
            if (desiredValue == null)
                return state.SelectedItem;

            if (state.DataSource is DataTable table)
            {
                foreach (DataRow row in table.Rows)
                {
                    object candidate = String.IsNullOrEmpty(state.ValueMember)
                        ? row
                        : row[state.ValueMember];
                    if (ValuesEqual(candidate, desiredValue))
                        return row;
                }

                return state.SelectedItem;
            }

            if (state.DataSource is IEnumerable enumerable)
            {
                foreach (object item in enumerable)
                {
                    object candidate = GetMemberValue(item, state.ValueMember);
                    if (String.IsNullOrEmpty(state.ValueMember))
                        candidate = GetMemberValue(item, "ID");

                    if (candidate != null && ValuesEqual(candidate, desiredValue))
                        return item;
                }
            }

            return state.SelectedItem;
        }

        private static string GetDisplayText(object item, string displayMember)
        {
            if (item == null)
                return String.Empty;

            if (item is DataRow row)
                return String.IsNullOrEmpty(displayMember) ? row.ToString() : Convert.ToString(row[displayMember]);

            if (item is DataRowView rowView)
                return String.IsNullOrEmpty(displayMember) ? rowView.ToString() : Convert.ToString(rowView[displayMember]);

            object value = GetMemberValue(item, displayMember);
            return value == null ? Convert.ToString(item) : Convert.ToString(value);
        }

        private static object GetMemberValue(object item, string memberName)
        {
            if (item == null || String.IsNullOrEmpty(memberName))
                return null;

            if (item is DataRow row && row.Table.Columns.Contains(memberName))
                return row[memberName];

            if (item is DataRowView rowView && rowView.DataView.Table.Columns.Contains(memberName))
                return rowView[memberName];

            PropertyInfo property = item.GetType().GetProperty(memberName, BindingFlags.Instance | BindingFlags.Public);
            return property == null ? null : property.GetValue(item, null);
        }

        private static NumericUpDown FindNumericCounterpart(ComboBox combo, List<Control> controls)
        {
            if (String.IsNullOrEmpty(combo.Name) || !combo.Name.StartsWith("cbx", StringComparison.Ordinal))
                return null;

            string numericName = "nud" + combo.Name.Substring(3);
            foreach (Control control in controls)
            {
                if (String.Equals(control.Name, numericName, StringComparison.Ordinal) && control is NumericUpDown numeric)
                    return numeric;
            }

            return null;
        }

        private static int GetItemCount(object source)
        {
            if (source is DataTable table)
                return table.Rows.Count;

            if (source is ICollection collection)
                return collection.Count;

            return 0;
        }

        private static bool ValuesEqual(object left, object right)
        {
            if (ReferenceEquals(left, right))
                return true;
            if (left == null || right == null || left == DBNull.Value || right == DBNull.Value)
                return false;

            return String.Equals(Convert.ToString(left), Convert.ToString(right), StringComparison.Ordinal);
        }

        private static bool IsDescendantOrSelf(Control control, Control root)
        {
            Control current = control;
            while (current != null)
            {
                if (ReferenceEquals(current, root))
                    return true;
                current = current.Parent;
            }

            return false;
        }

        private static void CollectControls(Control parent, List<Control> result)
        {
            foreach (Control child in parent.Controls)
            {
                result.Add(child);
                CollectControls(child, result);
            }
        }
    }

    internal static class ComboBoxControlListExtensions
    {
        public static IEnumerable<ComboBox> OfTypeComboBoxes(this IEnumerable<Control> controls)
        {
            foreach (Control control in controls)
            {
                if (control is ComboBox combo)
                    yield return combo;
            }
        }
    }
}
