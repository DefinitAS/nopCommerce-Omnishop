using OmnishopConnector.Model.SqlTypes;

namespace OmnishopConnector.Model
{
    /// <summary>
    ///     Refered to as "Bestillingskladd" in Anova Kasse
    /// </summary>
    public class tblPurchaseRequest
    {
        public virtual int Id { get; set; }

        public virtual int LineNo { get; set; }

        public virtual BKeyChar16 ProductId { get; set; }

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

        public virtual BKeyInt32? SupplierId { get; set; }

        public virtual tblSupplier navSupplier { get; set; }

        public virtual tblProduct navProduct { get; set; }
    }
}