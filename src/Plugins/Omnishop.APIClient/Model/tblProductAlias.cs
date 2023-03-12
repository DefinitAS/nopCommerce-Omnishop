using OmnishopConnector.Model.SqlTypes;
using static OmnishopConnector.Enums;

namespace OmnishopConnector.Model
{
    public class tblProductAlias
    {
        public virtual string Id { get; set; }

        public virtual BKeyChar16 ProductId { get; set; }

        public virtual tblProduct navProduct { get; set; }

        public int Priority { get; set; }

        public AliasTypes Type { get; set; }
    }
}