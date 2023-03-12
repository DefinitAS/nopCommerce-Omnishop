using static OmnishopConnector.Enums;

namespace OmnishopConnector.Model
{
    public class tblOrderDetailDiscount
    {
        public virtual int Id { get; set; }

        public virtual int OrderDetailId { get; set; }

        public virtual int Priority { get; set; }

        public DiscountOrigins Origin { get; set; }

        public virtual float DiscountPercent { get; set; }

        public virtual tblOrderDetail navOrderDetail { get; set; }
    }
}