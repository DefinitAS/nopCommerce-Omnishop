using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Payments.NetsEasy.Models
{
    public record ConfigurationModel : BaseNopModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Plugins.Payments.NetsEasy.Fields.MerchantId")]
        public string MerchantId { get; set; }
        public bool MerchantId_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.NetsEasy.Fields.Url")]
        public string Url { get; set; }
        public bool Url_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.NetsEasy.Fields.SecretKey")]
        public string SecretKey { get; set; }
        public bool SecretKey_OverrideForStore { get; set; }
        
        [NopResourceDisplayName("Plugins.Payments.NetsEasy.Fields.CheckoutKey")]
        public string CheckoutKey { get; set; }
        public bool CheckoutKey_OverrideForStore { get; set; }
    }
}