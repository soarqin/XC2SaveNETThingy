using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XC2SaveNETThingy
{
    public class PartyMember : IXC2SaveObject
    {
        public const int SIZE = 0x6;

        public UInt16 DriverId { get; set; }
        public Byte[] Unk_0x02 { get; set; }

        public PartyMember(Byte[] data)
        {
            DriverId = BitConverter.ToUInt16(data.GetByteSubArray(0, 2), 0);
            Unk_0x02 = data.GetByteSubArray(2, 4);
        }

        public Byte[] ToRawData()
        {
            List<Byte> result = new List<byte>();

            result.AddRange(BitConverter.GetBytes(DriverId));
            result.AddRange(Unk_0x02);

            if (result.Count != SIZE)
            {
                string message = "PartyMember: SIZE ALL WRONG!!!" + Environment.NewLine +
                "Size should be " + SIZE + " bytes..." + Environment.NewLine +
                "...but Size is " + result.Count + " bytes!";

                throw new Exception(message);
            }

            return result.ToArray();
        }

        public string ToString(XC2Data.LANG language)
        {
            return (string)XC2Data.Drivers(language).Select("Name WHERE ID = " + DriverId)[0][0];
        }

        public override string ToString()
        {
            return ToString(XC2Data.LANG.EN);
        }
    }
}
