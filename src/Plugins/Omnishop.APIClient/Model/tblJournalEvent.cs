using System;
using static OmnishopConnector.Enums;

namespace OmnishopConnector.Model
{
    public class tblJournalEvent
    {
        public virtual int Id { get; set; }

        public virtual string CashRegisterId { get; set; }

        public virtual int? EmployeeId { get; set; }

        public virtual DateTime OperationDateTime { get; set; }

        public virtual JournalOperationTypes OperationType { get; set; }

        public virtual string SerializedData { get; set; }

        public virtual decimal? Amount { get; set; }

        public virtual int? TenderDeclarationId { get; set; }

        public virtual tblEmployee navEmployee { get; set; }
        public virtual tblTenderDeclaration navTenderDeclaration { get; set; }
    }
}