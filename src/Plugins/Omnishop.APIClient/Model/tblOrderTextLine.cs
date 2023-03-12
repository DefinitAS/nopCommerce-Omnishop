namespace OmnishopConnector.Model
{
    public class tblOrderTextLine
    {
        public virtual int Id { get; set; }

        public virtual int OrderId { get; set; }

        public virtual int LineNo { get; set; }

        public virtual string Text { get; set; }

        public virtual tblOrder navOrder { get; set; }
    }
}