using System;
using OmnishopConnector.Model.SqlTypes;
using static OmnishopConnector.Enums;

namespace OmnishopConnector.Model
{
    public class tblSupplier
    {
        public virtual BKeyInt32 Id { get; set; }

        public virtual EntityStatuses Status { get; set; }

        public virtual string Name { get; set; }

        public virtual string Address { get; set; }

        public virtual string PostalCodeId { get; set; }

        public virtual string Phone { get; set; }

        public virtual string Email { get; set; }

        public virtual string Fax { get; set; }

        public virtual string Comment { get; set; }

        public virtual string DeliveryTerms { get; set; }

        public virtual PackageHandlingRules PackageHandlingRule { get; set; }

        public virtual string OurCustomerId { get; set; }

        public virtual DateTime DateCreated { get; set; }

        public virtual DateTime DateLastChange { get; set; }

        public virtual string ExtId1 { get; set; }

        public virtual string ExtId2 { get; set; }

        public virtual string ExtId3 { get; set; }

        public virtual string ExtId4 { get; set; }

        public virtual string ExtId5 { get; set; }
    }
}