using Be.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XC2SaveNETThingy
{
    public class FixedLengthByteProvider : IByteProvider
    {
        private Byte[] oldData;
        public Byte[] Data { get; set; }
        public long Length
        {
            get
            {
                return Data.Length;
            }
        }

        public FixedLengthByteProvider(Byte[] data)
        {
            oldData = data;
            Data = data;
        }

        public event EventHandler Changed;
        public event EventHandler LengthChanged;

        public byte ReadByte(long index)
        {
            return Data[index];
        }

        public void WriteByte(long index, byte value)
        {
            Data[index] = value;
        }

        public void InsertBytes(long index, byte[] bs)
        {
            throw new NotSupportedException();
        }

        public void DeleteBytes(long index, long length)
        {
            throw new NotSupportedException();
        }

        public bool HasChanges()
        {
            return (Data.Equals(oldData));
        }

        public void ApplyChanges()
        {
            throw new NotImplementedException();
        }

        public bool SupportsWriteByte()
        {
            return true;
        }

        public bool SupportsInsertBytes()
        {
            return false;
        }

        public bool SupportsDeleteBytes()
        {
            return false;
        }
    }
}
