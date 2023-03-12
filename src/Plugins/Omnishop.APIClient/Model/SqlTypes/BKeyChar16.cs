using System;
using System.IO;
using System.Text;

namespace OmnishopConnector.Model.SqlTypes
{
    public partial struct BKeyChar16 : IComparable<BKeyChar16>
    {
        private Int16 _bno;
        private string _bid;

        public BKeyChar16(Int16 bno, string bid)
        {
            if (string.IsNullOrEmpty(bid))
            {
                throw new ArgumentException("BId can not be null or empty.", "bid");
            }

            bid = bid.ToLower();
            if (!IsBIdValid(bid))
            {
                throw new ArgumentException("Invalid BId value: " + bid, "bid");
            }

            _bno = bno;
            _bid = bid;
        }

        public Int16 BNo
        {
            get { return _bno; }
            set { _bno = value; }
        }

        public string BId
        {
            get { return _bid; }
            set
            {
                var bid = value.ToLower();
                if (!IsBIdValid(bid))
                    throw new ArgumentException("Invalid BId value: " + bid);
                _bid = bid;
            }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is BKeyChar16))
                return false;

            var other = (BKeyChar16)obj;

            return this.BId == other.BId &&
                   this.BNo == other.BNo;
        }

        public int CompareTo(BKeyChar16 other)
        {
            return this.ToString().CompareTo(other.ToString());
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash + 23 * BNo.GetHashCode();
            if (BId != null)
                hash = hash + 23 * BId.GetHashCode();
            return hash;
        }

        public override string ToString()
        {
            if (_bno == 0)
                return _bid;

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

            var bidBytes = new byte[16];
            var idx = 0;
            while (idx < 15)
            {
                bidBytes[idx] = r.ReadByte();
                if (bidBytes[idx] == 0)
                    break;
                idx++;
            }

            _bid = Encoding.GetEncoding("IBM850").GetString(bidBytes, 0, idx);
        }

        public void Write(BinaryWriter w)
        {
            byte header = 0;
            w.Write(header);
            if (header == 1)
                return;

            var bidBytes = Encoding.GetEncoding("IBM850").GetBytes(_bid);
            var bidPaddingBytes = new byte[16 - bidBytes.Length];
            w.Write(_bno);
            w.Write(bidBytes);
            w.Write(bidPaddingBytes);
        }

        public static bool operator ==(BKeyChar16 c1, BKeyChar16 c2)
        {
            return c1.Equals(c2);
        }

        public static bool operator !=(BKeyChar16 c1, BKeyChar16 c2)
        {
            return !c1.Equals(c2);
        }

        public static explicit operator BKeyChar16(byte[] b)
        {
            var d = new BKeyChar16();
            using (var memstream = new MemoryStream(b))
            {
                using (var reader = new BinaryReader(memstream))
                    d.Read(reader);
                return d;
            }
        }

        public static BKeyChar16 FromString(string stringValue)
        {
            if (string.IsNullOrEmpty(stringValue))
                throw new ArgumentNullException("stringValue");

            var value = new BKeyChar16();
            string[] splittedValues = stringValue.Split('\\');
            if (splittedValues.Length == 1)
            {
                value.BNo = 0;
                value.BId = splittedValues[0];
            }
            else
            {
                value.BNo = Int16.Parse(splittedValues[0]);
                value.BId = splittedValues[1];
            }

            return value;
        }

        private static bool IsBIdValid(string lowerCaseValue)
        {
            return lowerCaseValue.Length < 16 && System.Text.RegularExpressions.Regex.IsMatch(lowerCaseValue, "^[æøå0-9a-z\\-/]+$");
        }

    }
}
