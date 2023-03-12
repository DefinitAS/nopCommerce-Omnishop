using static OmnishopConnector.Enums;

namespace OmnishopConnector.Model
{
    public class tblSaleDocumentDetailDiscount
    {
        public virtual int Id { get; set; }

        public virtual int SaleDocumentDetailId { get; set; }

        public virtual int Priority { get; set; }

        public DiscountOrigins Origin { get; set; }

        public virtual float DiscountPercent { get; set; }

        public virtual tblSaleDocumentDetail navSaleDocumentDetail { get; set; }
    }
}