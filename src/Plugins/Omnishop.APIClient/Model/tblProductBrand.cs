using System;
using static OmnishopConnector.Enums;

namespace OmnishopConnector.Model
{
    public class tblProductBrand
    {
        public virtual int Id { get; set; }
        public virtual EntityStatuses Status { get; set; }
        public virtual string Name { get; set; }
        public virtual string ExtId1 { get; set; }
        public virtual string ExtId2 { get; set; }
        public virtual string ExtId3 { get; set; }
        public virtual string ExtId4 { get; set; }
        public virtual string ExtId5 { get; set; }
        public virtual DateTime DateCreated { get; set; }
        public virtual DateTime DateLastChange { get; set; }
    }
}