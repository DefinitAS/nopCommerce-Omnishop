using System;
using System.Collections.Generic;
using OmnishopConnector.Model.SqlTypes;

namespace OmnishopConnector.Model
{
    public class tblTenderDeclaration
    {
        public virtual int Id { get; set; }

        public virtual Enums.TenderDeclarationStatusCodes Status { get; set; }

        public virtual string CashRegisterId { get; set; }

        public virtual DateTime DateTimeCreated { get; set; }
        public virtual DateTime? DateTimeCompleted { get; set; }

        public virtual BKeyChar16 EmployeeId { get; set; }

        public string Comment { get; set; }

        public string TerminalText { get; set; }

        public virtual tblEmployee navEmployee { get; set; }
        public virtual IList<tblTenderDeclarationAccount> navTenderDeclarationAccounts { get; set; }
    }
}