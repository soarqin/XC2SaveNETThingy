using System;
using System.Collections.Generic;
using System.Data;

namespace XC2SaveNETThingy
{
    /// <summary>
    /// Provides the small adapter needed by an unbound virtual item grid.
    /// The grid reads and writes the original save-file item array directly,
    /// so switching tabs does not allocate a DataGridView row object for every
    /// item slot.
    /// </summary>
    public sealed class ItemGridBinding
    {
        public Item[] Items { get; }
        public DataTable ItemNames { get; }
        public DataTable ItemTypes { get; }
        public Dictionary<UInt32, string> ItemNameLookup { get; }
        public Dictionary<UInt32, string> ItemTypeLookup { get; }

        public ItemGridBinding(Item[] items, DataTable itemNames, DataTable itemTypes)
        {
            Items = items ?? Array.Empty<Item>();
            ItemNames = itemNames;
            ItemTypes = itemTypes;
            ItemNameLookup = BuildLookup(itemNames);
            ItemTypeLookup = BuildLookup(itemTypes);
        }

        private static Dictionary<UInt32, string> BuildLookup(DataTable table)
        {
            Dictionary<UInt32, string> result = new Dictionary<UInt32, string>();
            if (table == null || !table.Columns.Contains("ID") || !table.Columns.Contains("Name"))
                return result;

            foreach (DataRow row in table.Rows)
            {
                if (UInt32.TryParse(Convert.ToString(row["ID"]), out UInt32 id))
                    result[id] = Convert.ToString(row["Name"]);
            }

            return result;
        }

        public object GetValue(int rowIndex, string propertyName)
        {
            Item item = Items[rowIndex];

            switch (propertyName)
            {
                case "ID":
                    return item.ID;
                case "Type":
                    return item.Type;
                case "Qty":
                    return item.Qty;
                case "Equipped":
                    return item.Equipped;
                case "Time_Hours":
                    return item.Time_Hours;
                case "Time_Minutes":
                    return item.Time_Minutes;
                case "Time_Seconds":
                    return item.Time_Seconds;
                case "Serial":
                    return item.Serial;
                case "Unk1":
                    return item.Unk1;
                case "Unk2":
                    return item.Unk2;
                case "Unk3":
                    return item.Unk3;
                default:
                    return null;
            }
        }

        public void SetValue(int rowIndex, string propertyName, object value)
        {
            if (value == null || value == DBNull.Value)
                return;

            Item item = Items[rowIndex];

            switch (propertyName)
            {
                case "ID":
                    item.ID = Convert.ToUInt16(value);
                    break;
                case "Type":
                    item.Type = Convert.ToByte(value);
                    break;
                case "Qty":
                    item.Qty = Convert.ToUInt16(value);
                    break;
                case "Equipped":
                    item.Equipped = Convert.ToBoolean(value);
                    break;
                case "Time_Hours":
                    item.Time_Hours = Convert.ToUInt32(value);
                    break;
                case "Time_Minutes":
                    item.Time_Minutes = Convert.ToByte(value);
                    break;
                case "Time_Seconds":
                    item.Time_Seconds = Convert.ToByte(value);
                    break;
                case "Serial":
                    item.Serial = Convert.ToUInt32(value);
                    break;
                case "Unk1":
                    item.Unk1 = Convert.ToBoolean(value);
                    break;
                case "Unk2":
                    item.Unk2 = Convert.ToBoolean(value);
                    break;
                case "Unk3":
                    item.Unk3 = Convert.ToByte(value);
                    break;
            }
        }
    }
}
