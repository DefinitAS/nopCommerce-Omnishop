using System;
using System.Collections.Generic;
using OmnishopConnector.Model.SqlTypes;

namespace OmnishopConnector.Model
{
    public class tblOrderDelivery
    {
        public virtual int Id { get; set; }
        public virtual string Comment { get; set; }
        public virtual string ConsignmentNo { get; set; }

        public virtual string TrackingUrl { get; set; }

        public virtual string LabelUrl { get; set; }

        public virtual string CashRegisterId { get; set; }
        public virtual DateTime DateCreated { get; set; }
        public virtual BKeyChar16 EmployeeId { get; set; }
        public virtual IList<tblOrderDeliveryDetail> navOrderDeliveryDetails { get; set; }
        public virtual tblEmployee navEmployee { get; set; }
    }
}