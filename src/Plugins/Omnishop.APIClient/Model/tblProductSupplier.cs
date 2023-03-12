using OmnishopConnector.Model.SqlTypes;

namespace OmnishopConnector.Model
{
    public class tblProductSupplier
    {
        public virtual BKeyInt32 SupplierId { get; set; }
        public virtual BKeyChar16 ProductId { get; set; }
        public virtual string SupplierProductId { get; set; }
        public virtual decimal Price { get; set; }
        public virtual float Discount1 { get; set; }
        public virtual float Discount2 { get; set; }
        public virtual int WholeSaleQuantity { get; set; }
        public virtual bool AllowBreakage { get; set; }
        public virtual decimal BreakageFee { get; set; }
        public virtual decimal NoBreakageDiscount { get; set; }
        public virtual int Priority { get; set; }
        public virtual tblProduct navProduct { get; set; }
        public virtual tblSupplier navSupplier { get; set; }
    }
}