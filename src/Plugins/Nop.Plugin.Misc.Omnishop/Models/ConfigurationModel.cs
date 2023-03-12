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


        
        [NopResourceDisplayName("Plugins.Misc.Omnishop.Fields.ApiUrlBase")]
        public string ApiUrlBase { get; set; }

        [NopResourceDisplayName("Plugins.Misc.Omnishop.Fields.ApiUser")]
        public string ApiUser { get; set; }

        [NopResourceDisplayName("Plugins.Misc.Omnishop.Fields.ApiPassword")]
        public string ApiPassword { get; set; }

        [NopResourceDisplayName("Plugins.Misc.Omnishop.Fields.DefaultClientId")]
        public short DefaultClientId { get; set; }

        [NopResourceDisplayName("Plugins.Misc.Omnishop.Fields.EmployeeId")]
        public string EmployeeId { get; set; }

        [NopResourceDisplayName("Plugins.Misc.Omnishop.Fields.ProductIdShipping")]
        public string ProductIdShipping { get; set; }

        [NopResourceDisplayName("Plugins.Misc.Omnishop.Fields.OriginCode")]
        public string OriginCode { get; set; }


    }
}