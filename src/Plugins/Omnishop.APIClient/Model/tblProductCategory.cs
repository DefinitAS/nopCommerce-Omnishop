using System;
using System.Collections.Generic;
using static OmnishopConnector.Enums;

namespace OmnishopConnector.Model
{
    public class tblProductCategory
    {
        public virtual int Id { get; set; }

        public virtual EntityStatuses Status { get; set; }

        public virtual string Name { get; set; }

        public virtual string Comment { get; set; }

        public virtual int? ParentId { get; set; }

        public virtual tblProductCategory navParent { get; set; }
        public virtual IList<tblProductCategory> navChildren { get; set; }


        public virtual string ExtId1 { get; set; }

        public virtual string ExtId2 { get; set; }

        public virtual string ExtId3 { get; set; }

        public virtual string ExtId4 { get; set; }

        public virtual string ExtId5 { get; set; }

        public virtual DateTime DateCreated { get; set; }

        public virtual DateTime DateLastChange { get; set; }
    }
}