using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XC2SaveNETThingy
{
    public class GameTime : IXC2SaveObject
    {
        public const int SIZE = 0x4;

        public Byte Seconds { get; set; }
        private const int SECONDS_BITS = 6;

        public Byte Minutes { get; set; }
        private const int MINUTES_BITS = 6;

        public Byte Hours { get; set; }
        private const int HOURS_BITS = 5;

        public UInt16 Days { get; set; }
        private const int DAYS_BITS = 15;

        public GameTime(Byte[] data)
        {
            UInt32 value = BitConverter.ToUInt32(data, 0);

            Days = (UInt16)(value >> (32 - DAYS_BITS));
            Hours = (Byte)((value >> (32 - DAYS_BITS - HOURS_BITS)) & (Byte)(Math.Pow(2, HOURS_BITS) - 1));
            Minutes = (Byte)((value >> (32 - DAYS_BITS - HOURS_BITS - MINUTES_BITS)) & (Byte)(Math.Pow(2, MINUTES_BITS) - 1));
            Seconds = (Byte)(value & (Byte)(Math.Pow(2, SECONDS_BITS) - 1));
        }

        public Byte[] ToRawData()
        {
            UInt32 result = (UInt32)((Days << (32 - DAYS_BITS)) + (Hours << (32 - DAYS_BITS - HOURS_BITS)) + (Minutes << SECONDS_BITS) + Seconds);

            return BitConverter.GetBytes(result);
        }
    }
}
