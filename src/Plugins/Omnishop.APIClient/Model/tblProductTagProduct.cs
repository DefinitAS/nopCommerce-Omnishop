using OmnishopConnector.Model.SqlTypes;

namespace OmnishopConnector.Model
{
    public class tblProductTagProduct
    {
        public virtual BKeyChar16 ProductId { get; set; }

        public virtual int ProductTagId { get; set; }

        public virtual tblProductTag navProductTag { get; set; }
        public virtual tblProduct navProduct { get; set; }
    }
}