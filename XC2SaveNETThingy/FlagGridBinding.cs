using System;
using System.Collections.Generic;

namespace XC2SaveNETThingy
{
    /// <summary>
    /// Adapter for a virtual flag grid. The save model stores wrapper objects,
    /// while DataGridView needs the wrapper's scalar Value property.
    /// </summary>
    public sealed class FlagGridBinding
    {
        private readonly Array values;

        public Dictionary<int, string> Names { get; }
        public int Count => values?.Length ?? 0;

        public FlagGridBinding(Array values, Dictionary<int, string> names)
        {
            this.values = values ?? Array.Empty<object>();
            Names = names;
        }

        public object GetValue(int index)
        {
            if (index < 0 || index >= Count)
                return null;

            object item = values.GetValue(index);
            if (item is FlagData.Flag_1 flag1)
                return flag1.Value;
            if (item is FlagData.Flag_2_4_8 flag248)
                return flag248.Value;
            if (item is FlagData.Flag_16 flag16)
                return flag16.Value;
            if (item is FlagData.Flag_32 flag32)
                return flag32.Value;

            return item;
        }

        public void SetValue(int index, object value)
        {
            if (index < 0 || index >= Count || value == null)
                return;

            object item = values.GetValue(index);
            if (item is FlagData.Flag_1 flag1)
                flag1.Value = Convert.ToBoolean(value);
            else if (item is FlagData.Flag_2_4_8 flag248)
                flag248.Value = Convert.ToByte(value);
            else if (item is FlagData.Flag_16 flag16)
                flag16.Value = Convert.ToUInt16(value);
            else if (item is FlagData.Flag_32 flag32)
                flag32.Value = Convert.ToUInt32(value);
            else
                values.SetValue(value, index);
        }
    }
}
