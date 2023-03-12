namespace OmnishopConnector.Model
{
    public class tblPaymentDetail
    {
        public virtual int Id { get; set; }

        public virtual int PaymentId { get; set; }

        public virtual int AccountId { get; set; }

        public virtual decimal Amount { get; set; }

        /// <summary>
        ///     Used when navAccount.PaymentRefNumberType = InternalPaymentId, points to record in tblTrackedPayment
        /// </summary>
        public virtual int? TrackedPaymentId { get; set; }

        /// <summary>
        ///     Used when navAccount.PaymentRefNumberType = ExternalRef
        /// </summary>

        public virtual string RefExternal { get; set; }

        public virtual string TerminalText { get; set; }

        public virtual tblPayment navPayment { get; set; }
        public virtual tblAccount navAccount { get; set; }
        public virtual tblTrackedPayment navTrackedPayment { get; set; }
    }
}