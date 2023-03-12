using System;

namespace OmnishopConnector.Model
{
    public class tblNumberSeries
    {
        public int Id { get; set; }
        public virtual string Table { get; set; }
        public virtual string Tag { get; set; }
        public virtual string Name { get; set; }
        public int SequenceStart { get; set; }
        public int SequenceEnd { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateLastChange { get; set; }
    }
}