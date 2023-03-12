using System;
using static OmnishopConnector.Enums;

namespace OmnishopConnector.Model
{
    public class tblCustomerCategory
    {
        public virtual int Id { get; set; }

        public virtual EntityStatuses Status { get; set; }

        public virtual string Name { get; set; }


        public virtual string Comment { get; set; }


        public virtual DateTime DateCreated { get; set; }


        public virtual DateTime DateLastChange { get; set; }
    }
}