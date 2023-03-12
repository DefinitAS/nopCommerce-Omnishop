using System;
using System.Collections.Generic;

namespace OmnishopConnector.Model
{
    public class tblTrackedPayment
    {
        public virtual int Id { get; set; }

        public virtual DateTime TransactionTime { get; set; }

        public virtual int AccountId { get; set; }

        public virtual decimal Amount { get; set; }

        public virtual int PaymentDetailId { get; set; }

        public virtual int? ParentTrackedPaymentId { get; set; }

        public virtual int IdentityCounter { get; set; }

        public virtual tblAccount navAccount { get; set; }
        public virtual tblTrackedPayment navParentTrackedPayment { get; set; }
        public virtual IList<tblTrackedPayment> navChildTrackedPayments { get; set; }
    }
}