using System.ComponentModel.DataAnnotations.Schema;
using OmnishopConnector.Model.SqlTypes;

namespace OmnishopConnector.Model
{
    public class tblStockBalance
    {
        public BKeyChar16 ProductId { get; set; }

        //[Range(-999999.999, 999999.999)]

        public virtual decimal Quantity { get; set; }

        //
        //public virtual decimal QuantityReserved { get; set; }

        //
        //public virtual decimal Delivered { get; set; }
        //
        //public virtual decimal Reserved { get; set; }


        [Column(TypeName = "decimal(19,2)")]
        public virtual decimal CostPrice { get; set; }

        public virtual tblProduct navProduct { get; set; }
    }
}