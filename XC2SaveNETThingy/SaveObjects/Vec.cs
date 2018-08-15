using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XC2SaveNETThingy
{
    public class Vec3 : IXC2SaveObject
    {
        public const int SIZE = 0xC;

        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public Vec3(float X = 0, float Y = 0, float Z = 0)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
        }

        public Vec3(Byte[] data)
        {
            X = BitConverter.ToSingle(data.GetByteSubArray(0, 4), 0);
            Y = BitConverter.ToSingle(data.GetByteSubArray(4, 4), 0);
            Z = BitConverter.ToSingle(data.GetByteSubArray(8, 4), 0);
        }

        public virtual Byte[] ToRawData()
        {
            List<Byte> result = new List<Byte>();

            result.AddRange(BitConverter.GetBytes(X));
            result.AddRange(BitConverter.GetBytes(Y));
            result.AddRange(BitConverter.GetBytes(Z));

            if (result.Count != SIZE)
            {
                string message = "Vec3: SIZE ALL WRONG!!!" + Environment.NewLine +
                "Size should be " + SIZE + " bytes..." + Environment.NewLine +
                "...but Size is " + result.Count + " bytes!";

                throw new Exception(message);
            }

            return result.ToArray();
        }
    }

    public class Vec3Padded : Vec3, IXC2SaveObject
    {
        public new const int SIZE = 0x10;

        public float Padding { get; set; }

        public Vec3Padded(float X = 0, float Y = 0, float Z = 0, float Padding = 0) : base(X, Y, Z)
        {
            this.Padding = Padding;
        }

        public Vec3Padded(Byte[] data) : base(data)
        {
            Padding = BitConverter.ToSingle(data.GetByteSubArray(0xC, 4), 0);
        }

        public override Byte[] ToRawData()
        {
            List<Byte> result = new List<Byte>();

            result.AddRange(base.ToRawData());
            result.AddRange(BitConverter.GetBytes(Padding));

            if (result.Count != SIZE)
            {
                string message = "Vec3Padded: SIZE ALL WRONG!!!" + Environment.NewLine +
                "Size should be " + SIZE + " bytes..." + Environment.NewLine +
                "...but Size is " + result.Count + " bytes!";

                throw new Exception(message);
            }

            return result.ToArray();
        }
    }
}
