using System;
using System.Collections.Generic;
using OmnishopConnector.Model.SqlTypes;
using static OmnishopConnector.Enums;

namespace OmnishopConnector.Model
{
    public class tblPayment
    {
        public virtual int Id { get; set; }

        public virtual PaymentTypes PaymentType { get; set; }

        public virtual BKeyChar16 EmployeeId { get; set; }

        public virtual int SaleDocumentId { get; set; }

        public virtual DateTime RegistrationDate { get; set; }

        public virtual DateTime? PaymentDate { get; set; }

        /// <summary>
        ///     Null if not (yet) part of tenderdeclaration
        ///     If payment is part of sale (PaymentType=WhenSold), this value should always be the same as for the sale.
        /// </summary>
        public virtual int? TenderDeclarationId { get; set; }

        public virtual string Comment { get; set; }

        public virtual string CashRegisterId { get; set; }

        public virtual IList<tblPaymentDetail> navPaymentDetails { get; set; }

        public virtual tblSaleDocument navSaleDocument { get; set; }

        public virtual tblEmployee navEmployee { get; set; }

        public virtual tblTenderDeclaration navTenderDeclaration { get; set; }
    }
}