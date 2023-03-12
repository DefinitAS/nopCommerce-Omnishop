using OmnishopConnector.Model.SqlTypes;

namespace OmnishopConnector.Model
{
    public class tblCampaignBundleProduct
    {
        public virtual int CampaignId { get; set; }
        public virtual int CampaignBundleNo { get; set; }
        public virtual BKeyChar16 ProductId { get; set; }
        public virtual int CampaignBundleComponentNo { get; set; }
        public virtual tblCampaignBundleComponent navCampaignBundleComponent { get; set; }
        public virtual tblProduct navProduct { get; set; }
    }
}