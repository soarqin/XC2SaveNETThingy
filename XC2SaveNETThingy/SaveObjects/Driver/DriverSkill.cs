using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XC2SaveNETThingy
{
    public class DriverSkill : IXC2SaveObject
    {
        public const int SIZE = 0xA;

        public UInt16[] IDs { get; set; }
        public UInt16 ColumnNo { get; set; }
        public UInt16 Level { get; set; }

        public DriverSkill(Byte[] data)
        {
            IDs = new UInt16[3];
            for (int i = 0; i < IDs.Length; i++)
                IDs[i] = BitConverter.ToUInt16(data.GetByteSubArray(i * 2, 2), 0);

            ColumnNo = BitConverter.ToUInt16(data.GetByteSubArray(0x6, 2), 0);
            Level = BitConverter.ToUInt16(data.GetByteSubArray(0x8, 2), 0);
        }

        public Byte[] ToRawData()
        {
            List<Byte> result = new List<byte>();

            foreach (UInt16 u in IDs)
                result.AddRange(BitConverter.GetBytes(u));

            result.AddRange(BitConverter.GetBytes(ColumnNo));
            result.AddRange(BitConverter.GetBytes(Level));

            if (result.Count != SIZE)
            {
                string message = "DriverSkill: SIZE ALL WRONG!!!" + Environment.NewLine +
                "Size should be " + SIZE + " bytes..." + Environment.NewLine +
                "...but Size is " + result.Count + " bytes!";

                throw new Exception(message);
            }

            return result.ToArray();
        }
    }
}
