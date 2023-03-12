using System;

namespace OmnishopConnector.Model
{
    public class tblJournalSale
    {
        public virtual int SaledocumentId { get; set; }

        public virtual DateTime TransactionDateTime { get; set; }

        public virtual decimal NetAmountIncVat { get; set; }

        public virtual decimal NetAmountExVat { get; set; }

        public virtual string Signature { get; set; }
        public virtual int KeyVersion { get; set; }

        public virtual string CashRegisterId { get; set; }

        public virtual tblSaleDocument navSaleDocument { get; set; }
    }
}