using System;
using System.Collections.Generic;
using OmnishopConnector.Model.SqlTypes;
using static OmnishopConnector.Enums;

namespace OmnishopConnector.Model
{
    public class tblQuote
    {
        public virtual int Id { get; set; }

        public virtual QuoteStatusCodes Status { get; set; }

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

        public virtual BKeyInt32 CustomerId { get; set; }

        public virtual IList<tblQuoteDetail> navQuoteDetails { get; set; }
        public virtual IList<tblQuoteTextLine> navQuoteTextLines { get; set; }
        public virtual tblCustomer navCustomer { get; set; }
        public virtual tblEmployee navEmployee { get; set; }
    }
}