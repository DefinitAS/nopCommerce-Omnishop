using OmnishopConnector.Model.SqlTypes;

namespace OmnishopConnector.Model
{
    public class tblPurchaseReceivementDetail
    {
        public virtual int Id { get; set; }

        public virtual int PurchaseReceivedId { get; set; }

        public virtual int LineNo { get; set; }

        public virtual BKeyChar16 ProductId { get; set; }

        public virtual int? PurchaseOrderDetailId { get; set; }

        public virtual string ProductName { get; set; }

        public virtual decimal Quantity { get; set; }

        /// <summary>
        ///     Purchase price for this purchase, before discounts are deducted. If no discounts, this should be same as
        ///     NetPriceExVat.
        /// </summary>
        public virtual decimal Price { get; set; }

        /// <summary>
        ///     Discount in percent of Price.
        /// </summary>
        public float Discount1 { get; set; }

        /// <summary>
        ///     Discount in percent of Price.
        /// </summary>
        public float Discount2 { get; set; }

        public virtual int VatRateId { get; set; }

        public virtual decimal VatRate { get; set; }

        public virtual string SupplierComment { get; set; }

        public virtual tblPurchaseReceivement navPurchaseReceivement { get; set; }
        public virtual tblProduct navProduct { get; set; }
        public virtual tblPurchaseOrderDetail navPurchaseOrderDetail { get; set; }
        public virtual tblVatRate navVatRate { get; set; }
    }
}