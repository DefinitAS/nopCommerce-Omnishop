using System;
using static OmnishopConnector.Enums;

namespace OmnishopConnector.Model
{
    public class tblQuantityUnit
    {
        public string Id { get; set; }

        public virtual decimal MinimumQuantity { get; set; } = 1;

        public virtual EntityStatuses Status { get; set; }

        public virtual string Name { get; set; }

        public virtual DateTime DateCreated { get; set; }

        public virtual DateTime DateLastChange { get; set; }
    }
}