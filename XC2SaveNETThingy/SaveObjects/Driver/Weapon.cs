using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XC2SaveNETThingy
{
    public class Weapon : IXC2SaveObject
    {
        public const int SIZE = 0x14;

        public UInt32[] ArtIds { get; set; }
        public UInt32 WeaponPoints { get; set; }
        public UInt32 TotalWeaponPoints { get; set; }

        public Weapon(Byte[] data)
        {
            ArtIds = new UInt32[3];
            for (int i = 0; i < ArtIds.Length; i++)
                ArtIds[i] = BitConverter.ToUInt32(data.GetByteSubArray(i * 4, 4), 0);

            WeaponPoints = BitConverter.ToUInt32(data.GetByteSubArray(0xC, 4), 0);
            TotalWeaponPoints = BitConverter.ToUInt32(data.GetByteSubArray(0x10, 4), 0);
        }

        public Byte[] ToRawData()
        {
            List<Byte> result = new List<Byte>();

            foreach (UInt32 u in ArtIds)
                result.AddRange(BitConverter.GetBytes(u));

            result.AddRange(BitConverter.GetBytes(WeaponPoints));
            result.AddRange(BitConverter.GetBytes(TotalWeaponPoints));

            if (result.Count != SIZE)
            {
                string message = "Weapon: SIZE ALL WRONG!!!" + Environment.NewLine +
                "Size should be " + SIZE + " bytes..." + Environment.NewLine +
                "...but Size is " + result.Count + " bytes!";

                throw new Exception(message);
            }

            return result.ToArray();
        }
    }
}
