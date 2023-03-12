using System;
using static OmnishopConnector.Enums;

namespace OmnishopConnector.Model
{
    public class tblUser
    {
        public virtual string Id { get; set; }

        public virtual EntityStatuses Status { get; set; }

        public virtual string Name { get; set; }

        public virtual string Email { get; set; }

        public virtual string Comment { get; set; }

        public virtual DateTime DateCreated { get; set; }

        public virtual DateTime DateLastChange { get; set; }
    }
}