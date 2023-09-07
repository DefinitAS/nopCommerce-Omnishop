using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Payments.CoinbaseCommerce.Models
{
    public record ConfigurationModel : BaseNopModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Plugins.Payments.CoinbaseCommerce.Fields.ApiKey")]
        public string ApiKey { get; set; }
        public bool ApiKey_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.CoinbaseCommerce.Fields.SharedSecretKey")]
        public string SharedSecretKey { get; set; }
        public bool SharedSecretKey_OverrideForStore { get; set; }

    }
}