namespace OmnishopConnector.Model
{
    public class tblQuoteTextLine
    {
        public virtual int Id { get; set; }

        public virtual int QuoteId { get; set; }

        public virtual int LineNo { get; set; }

        public virtual string Text { get; set; }

        public virtual tblQuote navQuote { get; set; }
    }
}