using System.Collections.Generic;
using OmnishopConnector.Model.SqlTypes;

namespace OmnishopConnector.Model
{
    public class tblOrderDetail
    {
        public virtual int Id { get; set; }
        public virtual int OrderId { get; set; }
        public virtual int LineNo { get; set; }
        public virtual BKeyChar16 ProductId { get; set; }
        public virtual string ProductName { get; set; }
        public virtual decimal Quantity { get; set; }

        /// <summary>
        ///     Price before discounts are deducted. If no discounts, this should be same as NetPriceExVat.
        /// </summary>
        public virtual decimal GrossPriceExVat { get; set; }

        /// <summary>
        ///     Price after discounts are deducted. If no discounts, this should be same as GrossPriceExVat.
        /// </summary>
        public virtual decimal NetPriceExVat { get; set; }

        public virtual int VatRateId { get; set; }
        public virtual tblOrder navOrder { get; set; }
        public virtual tblProduct navProduct { get; set; }
        public virtual tblVatRate navVatRate { get; set; }
        public virtual IList<tblOrderDeliveryDetail> navOrderDeliveryDetails { get; set; }
        public virtual IList<tblOrderDetailDiscount> navOrderDetailDiscounts { get; set; }
    }
}