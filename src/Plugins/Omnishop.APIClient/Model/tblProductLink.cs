namespace OmnishopConnector.Model
{
    public class tblProductLink
    {
        public virtual int Id { get; set; }

        public virtual int SourceProductId { get; set; }
        public virtual int DestProductId { get; set; }
        public virtual Enums.ProductLinkTypes ProductLinkType { get; set; }

        public virtual tblProduct navSourceProduct { get; set; }
        public virtual tblProduct navDestProduct { get; set; }
    }
}