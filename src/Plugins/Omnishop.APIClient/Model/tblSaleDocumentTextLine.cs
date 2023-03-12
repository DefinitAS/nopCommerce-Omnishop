namespace OmnishopConnector.Model
{
    public class tblSaleDocumentTextLine
    {
        public virtual int Id { get; set; }

        public virtual int SaleDocumentId { get; set; }

        public virtual int LineNo { get; set; }

        public virtual string Text { get; set; }

        public virtual tblSaleDocument navSaleDocument { get; set; }
    }
}