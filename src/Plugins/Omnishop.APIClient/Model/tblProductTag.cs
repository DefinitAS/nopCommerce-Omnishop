namespace OmnishopConnector.Model
{
    public class tblProductTag
    {
        public virtual int Id { get; set; }

        public virtual string Name { get; set; }

        public virtual int ProductTagTypeId { get; set; }

        public virtual tblProductTagType navProductTagType { get; set; }
    }
}