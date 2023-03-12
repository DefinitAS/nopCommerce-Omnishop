using System.Collections.Generic;
using static OmnishopConnector.Enums;

namespace OmnishopConnector.Model
{
    public class tblCampaignBundle
    {
        public virtual int CampaignId { get; set; }

        public virtual int CampaignBundleNo { get; set; }

        public virtual string Name { get; set; }

        public virtual CampaignBundleDiscountTypes DiscountType { get; set; }

        public virtual decimal? BundlePriceIncVAT { get; set; }

        public virtual tblCampaign navCampaign { get; set; }

        public virtual IList<tblCampaignBundleComponent> navCampaignBundleComponents { get; set; }
    }
}