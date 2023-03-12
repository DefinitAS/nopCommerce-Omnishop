using OmnishopConnector.Model.SqlTypes;

namespace OmnishopConnector.Model
{
    public class tblEmployeeLogin
    {
        public virtual BKeyChar16 EmployeeId { get; set; }

        public byte[] Password { get; private set; }

        public byte[] Salt { get; set; }

        public tblEmployee navEmployee { get; set; }
    }
}