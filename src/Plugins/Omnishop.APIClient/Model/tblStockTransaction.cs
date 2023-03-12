using System;
using OmnishopConnector.Model.SqlTypes;
using static OmnishopConnector.Enums;

namespace OmnishopConnector.Model
{
    public class tblStockTransaction
    {
        public int Id { get; set; }

        public BKeyChar16 ProductId { get; set; }

        public DateTime TransactionTime { get; set; }

        public virtual decimal QuantityDelta { get; set; }

        public StockTransactionOrigins Origin { get; set; }

        /// <summary>
        ///     Id of tblSaleDocumentDetail, tblStockTakingDetail or tblPurchaseReceivedDetail depending on Origin.
        /// </summary>
        public virtual int OriginDetailId { get; set; }

        /// <summary>
        ///     Id of tblSaleDocument, tblStockTakingDetail or tblPurchaseReceived depending on Origin.
        /// </summary>
        public virtual int OriginId { get; set; }

        public virtual tblProduct navProduct { get; set; }
    }
}