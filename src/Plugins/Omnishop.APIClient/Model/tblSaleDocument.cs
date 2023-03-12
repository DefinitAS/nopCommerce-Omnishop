using System;
using System.Collections.Generic;
using OmnishopConnector.Model.SqlTypes;
using static OmnishopConnector.Enums;

namespace OmnishopConnector.Model
{
    public class tblSaleDocument
    {
        public virtual int Id { get; set; }

        public virtual SaleDocumentTypes SaleDocumentType { get; set; }

        public virtual DateTime DateTimeCreated { get; set; }

        /// <summary>
        ///     Should only be set if SaleDocumentType is Invoice
        /// </summary>
        public virtual DateTime? DateInvoice { get; set; }

        public virtual DateTime? DateDue { get; set; }

        public virtual BKeyChar16 EmployeeId { get; set; }

        public virtual int? CustomerId { get; set; }

        public virtual string CustomerName { get; set; }

        public virtual string CustomerAddress { get; set; }

        public virtual string CustomerPostalCodeId { get; set; }

        public virtual string CustomerPhone { get; set; }

        public virtual string CustomerEmail { get; set; }

        public virtual string CustomerReference { get; set; }

        public virtual string CustomerOrderReference { get; set; }

        public virtual int? CustomerClubCardNo { get; set; }

        /// <summary>
        ///     Indicates that this sale is credited (canceled).
        ///     When a sale is credited a inverse sale of the original sale is created, marked with CreditedId pointing to this
        ///     sale.
        /// </summary>
        public virtual bool IsCredited { get; set; }

        /// <summary>
        ///     Indicates that this sale is a credit (cancelation) of sale with specified id.
        /// </summary>
        public virtual int? CreditOfSaleId { get; set; }

        /// <summary>
        ///     Null if sale is not (yet) part of tenderdeclaration
        /// </summary>
        public virtual int? TenderDeclarationId { get; set; }

        /// <summary>
        ///     Sum of all details, with all discounts subtracted and fees added. This is the amount the customer pays.
        /// </summary>
        public virtual decimal SaleSum { get; set; }

        public virtual PaymentStatuses PaymentStatus { get; set; }

        public virtual string CashRegisterId { get; set; }

        public virtual GeoCoordinate? LocationCoordinate { get; set; }

        public virtual string Comment { get; set; }

        public virtual IList<tblSaleDocumentDetail> navSaleDocumentDetails { get; set; }
        public virtual IList<tblSaleDocumentTextLine> navSaleDocumentTextLines { get; set; }
        public virtual IList<tblPayment> navPayments { get; set; }
        public virtual tblCustomer navCustomer { get; set; }
        public virtual tblEmployee navEmployee { get; set; }
        public virtual tblPostalCode navCustomerPostalCode { get; set; }
        public virtual tblTenderDeclaration navTenderDeclaration { get; set; }
    }
}