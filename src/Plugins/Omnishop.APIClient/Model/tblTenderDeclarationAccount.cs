namespace OmnishopConnector.Model
{
    public class tblTenderDeclarationAccount
    {
        public int Id { get; set; }

        public virtual int TenderDeclarationId { get; set; }
        public virtual int AccountId { get; set; }

        /// <summary>
        ///     Amount counted by cashier, read from terminal, etc.
        /// </summary>
        public virtual decimal AmountCounted { get; set; }

        /// <summary>
        ///     Amount withdrawn by cashier after counting, usually for deposit in bank/safe.
        /// </summary>
        public virtual decimal AmountWithdrawal { get; set; }

        public virtual decimal ClosingBalance => AmountCounted - AmountWithdrawal;

        public virtual tblTenderDeclaration navTenderDeclaration { get; set; }
        public virtual tblAccount navAccount { get; set; }
    }
}