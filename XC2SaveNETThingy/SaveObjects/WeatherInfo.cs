using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XC2SaveNETThingy
{
    public class WeatherInfo : IXC2SaveObject
    {
        public const int SIZE = 0x20;
        public const int COUNT = 64;

        public string Name { get; set; }
        public Byte[] Unk_0x04 { get; set; }

        public WeatherInfo(Byte[] data)
        {
            Name = Encoding.ASCII.GetString(data.GetByteSubArray(0x00, 4));
            Unk_0x04 = data.GetByteSubArray(0x04, 0x1C);
        }

        public Byte[] ToRawData()
        {
            List<Byte> result = new List<byte>();

            result.AddRange(Encoding.ASCII.GetBytes(Name));
            result.AddRange(Unk_0x04);

            if (result.Count != SIZE)
            {
                string message = "Weather: SIZE ALL WRONG!!!" + Environment.NewLine +
                "Size should be " + SIZE + " bytes..." + Environment.NewLine +
                "...but Size is " + result.Count + " bytes!";

                throw new Exception(message);
            }

            return result.ToArray();
        }
    }
}
