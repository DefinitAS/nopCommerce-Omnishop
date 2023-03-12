using System;
using OmnishopConnector.Model.SqlTypes;

namespace OmnishopConnector.Model
{
    public class tblEmployee
    {
        public virtual BKeyChar16 Id { get; set; }


        public virtual string Name { get; set; }

        public virtual string DisplayName { get; set; }


        public virtual string Address { get; set; }


        public virtual string PostalCodeId { get; set; }


        public virtual string Phone { get; set; }


        public virtual string Email { get; set; }


        public virtual string JobTitle { get; set; }


        public virtual string Comment { get; set; }


        public virtual string Region { get; set; }

        public virtual Enums.EntityStatuses Status { get; set; }

        public virtual DateTime DateCreated { get; set; }

        public virtual DateTime DateLastChange { get; set; }

        public virtual int EmployeeRoleId { get; set; }


        public virtual string ExtId1 { get; set; }

        public virtual tblEmployeeRole navEmployeeRole { get; set; }

        public string GetNameForDisplay()
        {
            return DisplayName ?? Name;
        }
    }
}