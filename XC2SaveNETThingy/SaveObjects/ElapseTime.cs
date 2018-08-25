using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XC2SaveNETThingy
{
    public class ElapseTime : IXC2SaveObject
    {
        public const int SIZE = 0x4;

        public Byte Seconds { get; set; }
        private const int SECONDS_BITS = 6;
        public Byte Minutes { get; set; }
        private const int MINUTES_BITS = 6;
        public UInt32 Hours { get; set; }
        private const int HOURS_BITS = 20;

        public ElapseTime(Byte[] data)
        {
            UInt32 value = BitConverter.ToUInt32(data, 0);

            Hours = (value >> ((SIZE * 8) - HOURS_BITS));
            Minutes = (Byte)((value >> ((SIZE * 8) - HOURS_BITS - MINUTES_BITS)) & (Byte)(Math.Pow(2, MINUTES_BITS) - 1));
            Seconds = (Byte)(value & (Byte)(Math.Pow(2, SECONDS_BITS) - 1));
        }

        public ElapseTime()
        {
            Hours = 0;
            Minutes = 0;
            Seconds = 0;
        }

        public Byte[] ToRawData()
        {
            return BitConverter.GetBytes((UInt32)((Hours << ((SIZE * 8) - HOURS_BITS)) + (Minutes << SECONDS_BITS) + Seconds));
        }

        public override string ToString()
        {
            return Hours + ":" + Minutes.ToString("D2") + ":" + Seconds.ToString("D2");
        }
    }
}
