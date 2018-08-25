using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XC2SaveNETThingy
{
    public class TaskAchieve : IXC2SaveObject
    {
        public const int SIZE = 0xC;
        public const int COUNT = 128;

        public Byte[] Unk { get; set; }

        public TaskAchieve(Byte[] data)
        {
            Unk = data;
        }

        public Byte[] ToRawData()
        {
            List<Byte> result = new List<byte>();

            result.AddRange(Unk);

            if (result.Count != SIZE)
            {
                string message = "TaskAchieve: SIZE ALL WRONG!!!" + Environment.NewLine +
                "Size should be " + SIZE + " bytes..." + Environment.NewLine +
                "...but Size is " + result.Count + " bytes!";

                throw new Exception(message);
            }

            return result.ToArray();
        }
    }
}
