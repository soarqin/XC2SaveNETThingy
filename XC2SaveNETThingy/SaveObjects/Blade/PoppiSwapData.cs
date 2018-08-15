using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XC2SaveNETThingy
{
    public class PoppiSwapData : IXC2SaveObject
    {
        public const int SIZE = 0x7c;

        private static readonly Dictionary<string, int> LOC = new Dictionary<string, int>()
        {
            { "RoleItem", 0x00 },
            { "AtrItem", 0x02 },
            { "BArtsEnhance", 0x04 },
            { "NArts", 0x28 },
            { "SkillRAMIDs", 0x4C },
            { "SkillUpgradeIDs", 0x52 },
            { "RoleCPUItemHandle", 0x58 },
            { "ElementCoreItemHandle", 0x5C },
            { "SkillRAMItemHandles", 0x60 },
            { "SkillUpgradeItemHandles", 0x6C },
            { "PowerCapacity", 0x78 },
            { "OpenCircuits", 0x7A }
        };

        public UInt16 RoleCPU { get; set; }
        public UInt16 ElementCore { get; set; }
        public ArtsEnhance[] BArtsEnhance { get; set; }
        public ArtsEnhance[] NArts { get; set; }
        public UInt16[] SkillRAMIDs { get; set; }
        public UInt16[] SkillUpgradeIDs { get; set; }
        public ItemHandle32 RoleCPUItemHandle { get; set; }
        public ItemHandle32 ElementCoreItemHandle { get; set; }
        public ItemHandle32[] SkillRAMItemHandles { get; set; }
        public ItemHandle32[] SkillUpgradeItemHandles { get; set; }
        public UInt16 PowerCapacity { get; set; }
        public HanaCircuits OpenCircuits { get; set; }

        public PoppiSwapData(Byte[] data)
        {
            RoleCPU = BitConverter.ToUInt16(data.GetByteSubArray(LOC["RoleItem"], 2), 0);
            ElementCore = BitConverter.ToUInt16(data.GetByteSubArray(LOC["AtrItem"], 2), 0);

            BArtsEnhance = new ArtsEnhance[3];
            for (int i = 0; i < BArtsEnhance.Length; i++)
                BArtsEnhance[i] = new ArtsEnhance(data.GetByteSubArray(LOC["BArtsEnhance"] + (i * ArtsEnhance.SIZE), ArtsEnhance.SIZE));

            NArts = new ArtsEnhance[3];
            for (int i = 0; i < NArts.Length; i++)
                NArts[i] = new ArtsEnhance(data.GetByteSubArray(LOC["NArts"] + (i * ArtsEnhance.SIZE), ArtsEnhance.SIZE));

            SkillRAMIDs = new UInt16[3];
            for (int i = 0; i < SkillRAMIDs.Length; i++)
                SkillRAMIDs[i] = BitConverter.ToUInt16(data.GetByteSubArray(LOC["SkillRAMIDs"] + (i * 2), 2), 0);

            SkillUpgradeIDs = new UInt16[3];
            for (int i = 0; i < SkillUpgradeIDs.Length; i++)
                SkillUpgradeIDs[i] = BitConverter.ToUInt16(data.GetByteSubArray(LOC["SkillUpgradeIDs"] + (i * 2), 2), 0);

            RoleCPUItemHandle = new ItemHandle32(data.GetByteSubArray(LOC["RoleCPUItemHandle"], ItemHandle32.SIZE));
            ElementCoreItemHandle = new ItemHandle32(data.GetByteSubArray(LOC["ElementCoreItemHandle"], ItemHandle32.SIZE));

            SkillRAMItemHandles = new ItemHandle32[3];
            for (int i = 0; i < SkillRAMItemHandles.Length; i++)
                SkillRAMItemHandles[i] = new ItemHandle32(data.GetByteSubArray(LOC["SkillRAMItemHandles"] + (i * ItemHandle32.SIZE), ItemHandle32.SIZE));

            SkillUpgradeItemHandles = new ItemHandle32[3];
            for (int i = 0; i < SkillUpgradeItemHandles.Length; i++)
                SkillUpgradeItemHandles[i] = new ItemHandle32(data.GetByteSubArray(LOC["SkillUpgradeItemHandles"] + (i * ItemHandle32.SIZE), ItemHandle32.SIZE));

            PowerCapacity = BitConverter.ToUInt16(data.GetByteSubArray(LOC["PowerCapacity"], 2), 0);
            OpenCircuits = new HanaCircuits(data.GetByteSubArray(LOC["OpenCircuits"], HanaCircuits.SIZE));
        }

        public Byte[] ToRawData()
        {
            List<Byte> result = new List<byte>();

            result.AddRange(BitConverter.GetBytes(RoleCPU));
            result.AddRange(BitConverter.GetBytes(ElementCore));

            foreach (ArtsEnhance ae in BArtsEnhance)
                result.AddRange(ae.ToRawData());

            foreach (ArtsEnhance ae in NArts)
                result.AddRange(ae.ToRawData());

            foreach (UInt16 u in SkillRAMIDs)
                result.AddRange(BitConverter.GetBytes(u));

            foreach (UInt16 u in SkillUpgradeIDs)
                result.AddRange(BitConverter.GetBytes(u));

            result.AddRange(RoleCPUItemHandle.ToRawData());
            result.AddRange(ElementCoreItemHandle.ToRawData());

            foreach (ItemHandle32 ih in SkillRAMItemHandles)
                result.AddRange(ih.ToRawData());

            foreach (ItemHandle32 ih in SkillUpgradeItemHandles)
                result.AddRange(ih.ToRawData());

            result.AddRange(BitConverter.GetBytes(PowerCapacity));
            result.AddRange(OpenCircuits.ToRawData());

            if (result.Count != SIZE)
            {
                string message = "Blade.Poppiswap: SIZE ALL WRONG!!!" + Environment.NewLine +
                    "Size should be " + SIZE + " bytes..." + Environment.NewLine +
                    "...but Size is " + result.Count + " bytes!";

                throw new Exception(message);
            }

            return result.ToArray();
        }
    }
}
