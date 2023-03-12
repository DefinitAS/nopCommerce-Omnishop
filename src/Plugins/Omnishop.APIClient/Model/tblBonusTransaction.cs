using System;
using OmnishopConnector.Model.SqlTypes;
using static OmnishopConnector.Enums;

namespace OmnishopConnector.Model
{
    public class tblBonusTransaction
    {
        public virtual int Id { get; set; }

        public virtual DateTime TransactionTime { get; set; }

        public virtual BKeyInt32 CustomerId { get; set; }

        public virtual decimal QuantityDelta { get; set; }

        public BonusTransactionOrigins Origin { get; set; }

        public virtual int OriginId { get; set; }

        public virtual int IdentityCounter { get; set; }

        public virtual tblCustomer navCustomer { get; set; }
    }
}