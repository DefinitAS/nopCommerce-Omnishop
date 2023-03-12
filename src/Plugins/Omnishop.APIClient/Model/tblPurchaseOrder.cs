using System;
using System.Collections.Generic;
using OmnishopConnector.Model.SqlTypes;
using static OmnishopConnector.Enums;

namespace OmnishopConnector.Model
{
    /// <summary>
    ///     When status is "Draft", purchaseorder can be freely deleted or edited : Add, remove, edit lines. Edit header
    ///     (comment, supplier, etc).
    ///     When purchaseorder is executed, edit (or delete) is not allowed.
    /// </summary>
    public class tblPurchaseOrder
    {
        public virtual int Id { get; set; }

        public virtual PurchaseOrderStatusCodes Status { get; set; }

        public virtual string Reference { get; set; }

        public virtual string Comment { get; set; }

        public virtual DateTime DateCreated { get; set; }

        public virtual DateTime DateLastChange { get; set; }

        /// <summary>
        ///     Should only be set when Status>=Executed
        /// </summary>
        public virtual DateTime? DateTimeExecuted { get; set; }

        /// <summary>
        ///     Upon creation this is the employee that created the PO.
        ///     After edit/save this is the employee that last edited.
        ///     After execution this is the employee that executed PO.
        /// </summary>
        public virtual BKeyChar16 EmployeeId { get; set; }

        public virtual BKeyInt32 SupplierId { get; set; }

        public virtual IList<tblPurchaseOrderDetail> navPurchaseOrderDetails { get; set; }
        public virtual IList<tblPurchaseOrderTextLine> navPurchaseOrderTextLines { get; set; }
        public virtual tblSupplier navSupplier { get; set; }
        public virtual tblEmployee navEmployee { get; set; }
    }
}