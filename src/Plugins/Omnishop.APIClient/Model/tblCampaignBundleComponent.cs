using System.Collections.Generic;

namespace OmnishopConnector.Model
{
    public class tblCampaignBundleComponent
    {
        public virtual int CampaignId { get; set; }
        public virtual int CampaignBundleNo { get; set; }
        public virtual int CampaignBundleComponentNo { get; set; }
        public virtual string Name { get; set; }
        public float Quantity { get; set; }
        public float DiscountPercent { get; set; }
        public float DiscountQuantity { get; set; }
        public virtual tblCampaignBundle navCampaignBundle { get; set; }
        public virtual IList<tblCampaignBundleProduct> navCampaignBundleProducts { get; set; }
    }
}