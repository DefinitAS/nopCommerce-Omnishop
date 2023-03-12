using System;
using System.Collections.Generic;
using static OmnishopConnector.Enums;

namespace OmnishopConnector.Model
{
    public class tblCampaign
    {
        public virtual int Id { get; set; }
        public virtual EntityStatuses Status { get; set; }
        public virtual string Name { get; set; }
        public virtual int Priority { get; set; }
        public virtual DateTime ValidFrom { get; set; }
        public virtual DateTime ValidTo { get; set; }
        public virtual DateTime DateCreated { get; set; }
        public virtual DateTime DateLastChange { get; set; }
        public virtual IList<tblCampaignProductPrice> navCampaignPrices { get; set; }
        public virtual IList<tblCampaignBundle> navCampaignBundles { get; set; }
    }
}