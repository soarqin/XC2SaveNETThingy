using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XC2SaveNETThingy
{
    public class RealTime : IXC2SaveObject
    {
        public const int SIZE = 0x8;
        // 13 bits
        public UInt16 Year { get; set; }
        private const int YEAR_BITS = 13;

        // 4 bits
        public Byte Month { get; set; }
        private const int MONTH_BITS = 4;

        // 9 bits
        public UInt16 UnkB { get; set; }
        private const int UNKB_BITS = 9;

        // 5 bits
        public Byte Day { get; set; }
        private const int DAY_BITS = 5;

        // 5 bits
        public Byte Hour { get; set; }
        private const int HOUR_BITS = 5;

        // 6 bits
        public Byte Minute { get; set; }
        private const int MINUTE_BITS = 6;

        // 6 bits 
        public Byte Second { get; set; }
        private const int SECOND_BITS = 6;

        // 10 bits
        public UInt16 MSecond { get; set; }
        private const int MSECOND_BITS = 10;

        // 6 bits
        public Byte UnkA { get; set; }
        private const int UNKA_BITS = 6;

        public RealTime(Byte[] data)
        {
            UInt64 value = BitConverter.ToUInt64(data, 0);

            Year = (UInt16)(value >> (64 - YEAR_BITS));
            Month = (Byte)((value << YEAR_BITS) >> (64 - MONTH_BITS));
            UnkB = (UInt16)((value << (YEAR_BITS + MONTH_BITS)) >> (64 - UNKB_BITS));
            Day = (Byte)((value << (YEAR_BITS + MONTH_BITS + UNKB_BITS)) >> (64 - DAY_BITS));
            Hour = (Byte)((value << (YEAR_BITS + MONTH_BITS + UNKB_BITS + DAY_BITS)) >> (64 - HOUR_BITS));
            Minute = (Byte)((value << (YEAR_BITS + MONTH_BITS + UNKB_BITS + DAY_BITS + HOUR_BITS)) >> (64 - MINUTE_BITS));
            Second = (Byte)((value << (YEAR_BITS + MONTH_BITS + UNKB_BITS + DAY_BITS + HOUR_BITS + MINUTE_BITS)) >> (64 - SECOND_BITS));
            MSecond = (UInt16)((value << (YEAR_BITS + MONTH_BITS + UNKB_BITS + DAY_BITS + HOUR_BITS + MINUTE_BITS + SECOND_BITS)) >> (64 - MSECOND_BITS));
            UnkA = (Byte)((value << (YEAR_BITS + MONTH_BITS + UNKB_BITS + DAY_BITS + HOUR_BITS + MINUTE_BITS + SECOND_BITS + MSECOND_BITS)) >> (64 - UNKA_BITS));
        }

        public Byte[] ToRawData()
        {
            UInt64 derp = 0;

            derp += (UInt64)Year << (64 - YEAR_BITS);
            derp += (UInt64)Month << (64 - YEAR_BITS - MONTH_BITS);
            derp += (UInt64)UnkB << (64 - YEAR_BITS - MONTH_BITS - UNKB_BITS);
            derp += (UInt64)Day << (64 - YEAR_BITS - MONTH_BITS - UNKB_BITS - DAY_BITS);
            derp += (UInt64)Hour << (64 - YEAR_BITS - MONTH_BITS - UNKB_BITS - DAY_BITS - HOUR_BITS);
            derp += (UInt64)Minute << (64 - YEAR_BITS - MONTH_BITS - UNKB_BITS - DAY_BITS - HOUR_BITS - MINUTE_BITS);
            derp += (UInt64)Second << (64 - YEAR_BITS - MONTH_BITS - UNKB_BITS - DAY_BITS - HOUR_BITS - MINUTE_BITS - SECOND_BITS);
            derp += (UInt64)MSecond << (64 - YEAR_BITS - MONTH_BITS - UNKB_BITS - DAY_BITS - HOUR_BITS - MINUTE_BITS - SECOND_BITS - MSECOND_BITS);
            derp += (UInt64)UnkA;

            return BitConverter.GetBytes(derp);
        }

        public override string ToString()
        {
            return String.Format("{0:0000}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}.{6:000}", Year, Month, Day, Hour, Minute, Second, MSecond);
        }
    }
}
