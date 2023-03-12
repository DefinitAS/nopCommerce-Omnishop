using System;
using System.Collections.Generic;
using OmnishopConnector.Model.SqlTypes;
using static OmnishopConnector.Enums;

namespace OmnishopConnector.Model
{
    public class tblOrder
    {
        public virtual int Id { get; set; }
        public virtual OrderStatusCodes Status { get; set; }
        public virtual string Comment { get; set; }
        public virtual DateTime DateCreated { get; set; }
        public virtual DateTime DateLastChange { get; set; }

        /// <summary>
        ///     Should only be set when Status>=Completed
        /// </summary>
        public virtual DateTime? DateTimeCompleted { get; set; }

        /// <summary>
        ///     Upon creation this is the employee that created the PO.
        ///     After edit/save this is the employee that last edited.
        ///     After execution this is the employee that executed PO.
        /// </summary>
        public virtual BKeyChar16 EmployeeId { get; set; }

        public virtual BKeyInt32 CustomerId { get; set; }
        public virtual string CustomerName { get; set; }
        public virtual string CustomerAddress { get; set; }
        public virtual string CustomerPostalCodeId { get; set; }
        public virtual string CustomerPhone { get; set; }
        public virtual string CustomerEmail { get; set; }
        public virtual string CustomerReference { get; set; }
        public virtual string CustomerOrderReference { get; set; }
        public virtual string ExternalId { get; set; }
        public virtual string ExternalOrigin { get; set; }
        public virtual string ExternalPaymentMethod { get; set; }
        public virtual string ExternalShippingMethod { get; set; }
        public virtual IList<tblOrderDetail> navOrderDetails { get; set; }
        public virtual IList<tblOrderTextLine> navOrderTextLines { get; set; }
        public virtual tblCustomer navCustomer { get; set; }
        public virtual tblEmployee navEmployee { get; set; }
        public virtual tblPostalCode navCustomerPostalCode { get; set; }
    }
}