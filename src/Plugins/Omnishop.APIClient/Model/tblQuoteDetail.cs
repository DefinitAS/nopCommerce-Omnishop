using OmnishopConnector.Model.SqlTypes;

namespace OmnishopConnector.Model
{
    public class tblQuoteDetail
    {
        public virtual int Id { get; set; }

        public virtual int QuoteId { get; set; }

        public virtual int LineNo { get; set; }

        public virtual BKeyChar16 ProductId { get; set; }

        public virtual string ProductName { get; set; }

        public virtual decimal Quantity { get; set; }

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

        public virtual tblQuote navQuote { get; set; }
        public virtual tblProduct navProduct { get; set; }
        public virtual tblVatRate navVatRate { get; set; }
    }
}