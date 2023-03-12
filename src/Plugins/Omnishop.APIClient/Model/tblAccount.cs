using static OmnishopConnector.Enums;

namespace OmnishopConnector.Model
{
    public class tblAccount
    {
        public virtual int Id { get; set; }

        public virtual EntityStatuses Status { get; set; }

        public virtual AccountTypes AccountType { get; set; }

        public virtual string Name { get; set; }

        /// <summary>
        ///     Name to be used when amount is negative (for account with ordinary name of "kontant" this will be "tilbake til
        ///     kunde")
        /// </summary>

        public virtual string PaymentTypeNameNegative { get; set; }


        public virtual PaymentRefNumberTypes PaymentRefNumberType { get; set; }

        /// <summary>
        ///     RegEx for validation of RefExternal of PaymentDetail, when PaymentRefNumberType=ExternalRef
        /// </summary>

        public virtual string PaymentRefExternalRegEx { get; set; }

        /// <summary>
        ///     Indicates that cash drawer should be opened when registering payments against this account.
        /// </summary>
        public virtual bool OpensCashDrawer { get; set; }

        public virtual bool HasBalance { get; set; }

        /// <summary>
        ///     Prefix to use when printing EAN codes that represents this account (used for printing and scanning
        ///     gavekort/tilgodeseddel receipts)
        /// </summary>
        public virtual string EANPrefix { get; set; }

        /// <summary>
        ///     Payment account that can not be used at initial sale, but can be used for payments of invoices.
        /// </summary>
        public virtual bool NotAtSale { get; set; }

        /// <summary>
        ///     For PaymentTerminal accounts. This is the IssuerId as used by the terminal. Allows of mapping
        ///     of transactions from terminal to correct account.
        /// </summary>
        public virtual int? IssuerId { get; set; }


        public virtual string IssuerName { get; set; }


        public virtual string ExtId1 { get; set; }

        public virtual string ExtId2 { get; set; }

        public virtual string ExtId3 { get; set; }

        public virtual string ExtId4 { get; set; }

        public virtual string ExtId5 { get; set; }
    }
}