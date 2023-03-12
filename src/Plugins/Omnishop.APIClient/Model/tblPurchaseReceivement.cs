using System;
using System.Collections.Generic;
using OmnishopConnector.Model.SqlTypes;
using static OmnishopConnector.Enums;

namespace OmnishopConnector.Model
{
    public class tblPurchaseReceivement
    {
        public virtual int Id { get; set; }

        public virtual int PurchaseOrderId { get; set; }

        public virtual PurchaseReceivedStatusCodes Status { get; set; }

        public virtual PurchaseReceivedInvoiceStatusCodes InvoiceReceivedStatus { get; set; }

        public virtual string Comment { get; set; }

        public virtual DateTime Date { get; set; }

        public virtual BKeyChar16 EmployeeId { get; set; }

        public virtual string SupplierOrderNo { get; set; }

        public virtual string SupplierRef { get; set; }

        public virtual DateTime? SupplierInvoiceDate { get; set; }

        public virtual string SupplierInvoiceNo { get; set; }

        public virtual IList<tblPurchaseReceivementDetail> navPurchaseReceivementDetails { get; set; }
        public virtual tblPurchaseOrder navPurchaseOrder { get; set; }
        public virtual tblEmployee navEmployee { get; set; }
    }
}