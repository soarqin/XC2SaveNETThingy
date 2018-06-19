using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XC2SaveNETThingy
{
    public static class Utils
    {
        public static Byte[] GetByteSubArray(this Byte[] data, int startLoc, int length)
        {
            Byte[] value = new Byte[length];

            for (int i = 0; i < value.Length; i++)
            {
                value[i] = data[startLoc + i];
            }

            return value;
        }

    }
}
