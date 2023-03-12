using OmnishopConnector.Model.SqlTypes;

namespace OmnishopConnector.Model
{
    public class tblStockTakingDetail
    {
        public virtual int Id { get; set; }

        public virtual int StockTakingId { get; set; }

        public virtual int LineNo { get; set; }

        public virtual BKeyChar16 ProductId { get; set; }

        public virtual string ProductName { get; set; }

        /// <summary>
        ///     This is the original stockbalance at the time this line was added to StockTaking
        /// </summary>

        public virtual decimal OriginalQuantity { get; set; }

        /// <summary>
        ///     null means 'Not counted'.
        /// </summary>

        public virtual decimal? CountedQuantity { get; set; }

        public virtual tblStockTaking navStockTaking { get; set; }
        public virtual tblProduct navProduct { get; set; }
    }
}