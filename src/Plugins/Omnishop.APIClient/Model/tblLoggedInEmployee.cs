using System;
using OmnishopConnector.Model.SqlTypes;

namespace OmnishopConnector.Model
{
    public class tblLoggedInEmployee
    {
        public virtual int Id { get; set; }

        public virtual BKeyChar16 EmployeeId { get; set; }

        public DateTime LoginTime { get; set; }

        public virtual string CashRegisterId { get; set; }

        public virtual tblEmployee navEmployee { get; set; }
    }
}