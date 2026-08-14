using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XC2SaveNETThingy
{
    public static class Utils
    {
        public static Byte[] GetByteSubArray(this Byte[] data, int startLoc, int length)
        {
            Byte[] value = new Byte[length];

            for (int i = 0; i < value.Length; i++)
                value[i] = data[startLoc + i];

            return value;
        }

        public static int SanitizeCbxIndex(this ComboBox box, int value)
        {
            if (value < 0 || value > box.Items.Count)
                return 0;
            else
                return value;
        }

        public static Byte SanitizeByte(this Byte value)
        {
            if (value < 0 || value > Byte.MaxValue)
                return 0;
            else
                return value;
        }

        public static Byte SanitizeByte(this int value)
        {
            if (value < 0 || value > Byte.MaxValue)
                return 0;
            else
                return (Byte)value;
        }

        public static UInt16 SanitizeUInt16(this UInt16 value)
        {
            if (value < 0 || value > UInt16.MaxValue)
                return 0;
            else
                return value;
        }

        public static UInt16 SanitizeUInt16(this int value)
        {
            if (value < 0 || value > UInt16.MaxValue)
                return 0;
            else
                return (UInt16)value;
        }

        public static Int32 SanitizeInt32(this Int32 value)
        {
            if (value < 0 || value > Int32.MaxValue)
                return 0;
            else
                return value;
        }

        public static UInt32 SanitizeUInt32(this UInt32 value)
        {
            if (value < 0 || value > UInt32.MaxValue)
                return 0;
            else
                return value;
        }

        public static UInt64 SanitizeUInt16(this UInt64 value)
        {
            if (value < 0 || value > UInt64.MaxValue)
                return 0;
            else
                return value;
        }

        public static void BindDataTable(this ComboBox cbx, DataTable dataSource, string displayMember = "Name", string valueMember = "ID")
        {
            cbx.DisplayMember = displayMember;
            cbx.ValueMember = valueMember;
            cbx.DataSource = new DataView(dataSource);
        }
    }
}
