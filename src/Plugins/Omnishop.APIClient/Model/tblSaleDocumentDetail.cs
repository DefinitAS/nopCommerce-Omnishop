using System.Collections.Generic;
using OmnishopConnector.Model.SqlTypes;

namespace OmnishopConnector.Model
{
    public class tblSaleDocumentDetail
    {
        public virtual int Id { get; set; }

        public virtual int SaleDocumentId { get; set; }

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

        /// <summary>
        ///     The cost of product(s) on this line. Used to calculate markup amount.
        /// </summary>
        public virtual decimal CostPrice { get; set; }

        public virtual int VatRateId { get; set; }

        public virtual decimal VatRate { get; set; }

        /// <summary>
        ///     This is the ordinary price (ex. VAT) at the time of sale. If no discounts, campaigns or special pricing is applied
        ///     this is the same as Price.
        /// </summary>
        public virtual decimal RRP { get; set; }

        public virtual tblSaleDocument navSaleDocument { get; set; }
        public virtual tblProduct navProduct { get; set; }
        public virtual tblVatRate navVatRate { get; set; }

        public virtual IList<tblSaleDocumentDetailDiscount> navSaleDocumentDetailDiscounts { get; set; }
    }
}