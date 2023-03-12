namespace OmnishopConnector.Model
{
    public class tblPurchaseOrderTextLine
    {
        public virtual int Id { get; set; }

        public virtual int PurchaseOrderId { get; set; }

        public virtual int LineNo { get; set; }

        public virtual string Text { get; set; }

        public virtual tblPurchaseOrder navPurchaseOrder { get; set; }
    }
}