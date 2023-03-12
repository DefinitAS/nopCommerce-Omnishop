using OmnishopConnector.Model.SqlTypes;

namespace OmnishopConnector.Model
{
    public class tblCampaignProductPrice
    {
        public virtual int CampaignId { get; set; }
        public virtual BKeyChar16 ProductId { get; set; }
        public virtual decimal Price { get; set; }
        public virtual tblCampaign navCampaign { get; set; }
        public virtual tblProduct navProduct { get; set; }
    }
}