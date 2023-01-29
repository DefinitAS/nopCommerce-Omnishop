using System.Collections.Generic;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.Omnishop.Models
{
    public record ConfigurationModel : BaseNopModel
    {
        public ConfigurationModel()
        {

        }

        public int ActiveStoreScopeConfiguration { get; set; }


        [NopResourceDisplayName("Plugins.Misc.Omnishop.Fields.ForceCanonicalUrl")]
        public bool ForceCanonicalUrl { get; set; }

        [NopResourceDisplayName("Plugins.Misc.Omnishop.Fields.ProductImportApiKey")]
        public string ProductImportApiKey { get; set; }

    }
}