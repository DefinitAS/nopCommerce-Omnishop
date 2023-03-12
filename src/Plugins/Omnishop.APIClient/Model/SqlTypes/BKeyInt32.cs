using System;
using System.IO;
using System.Text;

namespace OmnishopConnector.Model.SqlTypes
{
    public partial struct BKeyInt32 : IComparable<BKeyInt32>
    {
        private Int16 _bno;
        private Int32 _bid;

        public static bool operator ==(BKeyInt32 c1, BKeyInt32 c2)
        {
            return c1.Equals(c2);
        }

        public static bool operator !=(BKeyInt32 c1, BKeyInt32 c2)
        {
            return !c1.Equals(c2);
        }

        public static explicit operator BKeyInt32(byte[] b)
        {
            BKeyInt32 d = new BKeyInt32();
            using (var memstream = new MemoryStream(b))
            {
                using (var reader = new BinaryReader(memstream))
                    d.Read(reader);
                return d;
            }
        }

        public Int16 BNo
        {
            get { return _bno; }
            set { _bno = value; }
        }

        public Int32 BId
        {
            get { return _bid; }
            set { _bid = value; }
        }

        public BKeyInt32(Int16 bno, Int32 bid)
        {
            _bno = bno;
            _bid = bid;
        }

        public static BKeyInt32 FromString(string stringValue)
        {
            if (string.IsNullOrEmpty(stringValue))
                throw new ArgumentNullException("stringValue");

            var value = new BKeyInt32();
            string[] splittedValues = stringValue.Split('\\');
            if (splittedValues.Length == 1)
            {
                value.BNo = 0;
                value.BId = Int32.Parse(splittedValues[0]);
            }
            else
            {
                value.BNo = Int16.Parse(splittedValues[0]);
                value.BId = Int32.Parse(splittedValues[1]);
            }

            return value;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is BKeyInt32))
                return false;

            var other = (BKeyInt32)obj;

            return this.BId == other.BId &&
                   this.BNo == other.BNo;
        }

        public int CompareTo(BKeyInt32 other)
        {
            return this.ToString().CompareTo(other.ToString());
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash + 23 * BNo.GetHashCode();
            hash = hash + 23 * BId.GetHashCode();
            return hash;
        }

        public override string ToString()
        {
            if (_bno == 0)
                return _bid.ToString();

            var builder = new StringBuilder();
            builder.Append(_bno);
            builder.Append(@"\");
            builder.Append(_bid);
            return builder.ToString();
        }

        public void Read(BinaryReader r)
        {
            byte header = r.ReadByte();

            if ((header & 1) > 0)
            {
                throw new InvalidDataException("Null values can not be read");
            }

            _bno = r.ReadInt16();
            _bid = r.ReadInt32();
        }

        public void Write(BinaryWriter w)
        {
            byte header = 0;
            w.Write(header);
            if (header == 1)
                return;

            w.Write(_bno);
            w.Write(_bid);
        }

    }
}
