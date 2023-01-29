using Nop.Core.Configuration;

namespace Nop.Plugin.Misc.Omnishop.Models
{
    public class OmnishopPluginSettings : ISettings
    {
        /// <summary>
        /// If this is true, all requests to hosts other than the one defined in Store configuration, will be redirected to the host defined in Store
        /// </summary>
        public bool ForceCanonicalUrl { get; set; }

        public string ProductImportApiKey { get; set; }

    }
}
