using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XC2SaveNETThingy
{
    public interface IXC2SaveObject
    {
        Byte[] ToRawData();
    }
}
