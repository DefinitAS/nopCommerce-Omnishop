using System;
using OmnishopConnector.Model.SqlTypes;
using static OmnishopConnector.Enums;

namespace OmnishopConnector.Model
{
    public class tblCustomer
    {
        public virtual BKeyInt32 Id { get; set; }

        public virtual string Name { get; set; }


        public virtual string Address { get; set; }


        public virtual string PostalCodeId { get; set; }


        public virtual string Phone { get; set; }


        public virtual string Email { get; set; }


        public virtual string Comment { get; set; }

        public virtual float Discount { get; set; }

        public virtual int? CustomerCategoryId { get; set; }


        public string OrganizationNo { get; set; }

        public virtual CreditStatuses CreditStatus { get; set; }

        /// <summary>
        ///     If not set system default will be used/suggested.
        /// </summary>
        public virtual int? InvoiceDueDays { get; set; }


        public virtual string BillingName { get; set; }


        public virtual string BillingAddress { get; set; }


        public virtual string BillingPostalCodeId { get; set; }


        public virtual string BillingReference { get; set; }


        public virtual DateTime DateCreated { get; set; }


        public virtual DateTime DateLastChange { get; set; }

        public virtual EntityStatuses Status { get; set; }


        public virtual string ExternalId { get; set; }


        public virtual string ExternalOrigin { get; set; }


        public virtual string ExtId1 { get; set; }

        public virtual string ExtId2 { get; set; }

        public virtual string ExtId3 { get; set; }

        public virtual string ExtId4 { get; set; }

        public virtual string ExtId5 { get; set; }


        public virtual tblCustomerCategory navCustomerCategory { get; set; }
        public virtual tblPostalCode navPostalCode { get; set; }
        public virtual tblPostalCode navBillingPostalCode { get; set; }
    }
}