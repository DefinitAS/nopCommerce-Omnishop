using System;
using System.Collections.Generic;
using OmnishopConnector.Model.SqlTypes;
using static OmnishopConnector.Enums;

namespace OmnishopConnector.Model
{
    public class tblStockTaking
    {
        public virtual int Id { get; set; }

        public virtual StockTakingStatusCodes Status { get; set; }


        public virtual string Comment { get; set; }

        public virtual DateTime Date { get; set; }

        public virtual BKeyChar16 EmployeeId { get; set; }

        public virtual IList<tblStockTakingDetail> navStockTakingDetails { get; set; }
        public virtual tblEmployee navEmployee { get; set; }
    }
}