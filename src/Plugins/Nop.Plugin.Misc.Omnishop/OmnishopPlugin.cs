using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Nop.Core;
using Nop.Services.Common;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Menu;

namespace Nop.Plugin.Misc.Omnishop
{
    public partial class OmnishopPlugin : BasePlugin, IAdminMenuPlugin, IMiscPlugin
    {
        private readonly IPermissionService _permissionService;
        private readonly IWebHelper _webHelper;
        private readonly ILocalizationService _localizationService;

        public OmnishopPlugin(IPermissionService permissionService,
            IWebHelper webHelper,
            ILocalizationService localizationService)
        {
            _permissionService = permissionService;
            _webHelper = webHelper;
            _localizationService = localizationService;
        }

        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/OmnishopPluginConfiguration/Configure";
        }

        public override async Task InstallAsync()
        {
            //locales
            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                ["Plugins.Misc.Omnishop.Fields.ForceCanonicalUrl"] = "Force canonical url",
                ["Plugins.Misc.Omnishop.Fields.ProductImportApiKey"] = "API Key for Productimport services",
            });

            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                ["Plugins.Misc.Omnishop.PublishProducts.List.SearchHasImages"] = "Images",
                ["Plugins.Misc.Omnishop.PublishProducts.List.SearchHasImages.Has"] = "Has image(s)",
                ["Plugins.Misc.Omnishop.PublishProducts.List.SearchHasImages.HasNot"] = "Has no images",

                ["Plugins.Misc.Omnishop.PublishProducts.List.SearchHasShortDescription"] = "Short description",
                ["Plugins.Misc.Omnishop.PublishProducts.List.SearchHasShortDescription.Has"] = "Has short description",
                ["Plugins.Misc.Omnishop.PublishProducts.List.SearchHasShortDescription.HasNot"] = "No short description",

                ["Plugins.Misc.Omnishop.PublishProducts.List.SearchHasLongDescription"] = "Has long description",
                ["Plugins.Misc.Omnishop.PublishProducts.List.SearchHasLongDescription.Has"] = "Has long description",
                ["Plugins.Misc.Omnishop.PublishProducts.List.SearchHasLongDescription.HasNot"] = "No long description",

                ["Plugins.Misc.Omnishop.PublishProducts.List.SearchPriceNotZero"] = "Price",
                ["Plugins.Misc.Omnishop.PublishProducts.List.SearchPriceNotZero.Has"] = "Price >0",
                ["Plugins.Misc.Omnishop.PublishProducts.List.SearchHasImage.HasNot"] = "Price <=0",
            });

            
            await base.InstallAsync();
        }

        public async Task ManageSiteMapAsync(SiteMapNode rootNode)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return;

            //Insert Admin menu item for "Publish products"
            rootNode
                .ChildNodes.FirstOrDefault(n => n.SystemName == "Catalog")
                .ChildNodes.Add(new SiteMapNode()
                {
                    SystemName = "Misc.Omnishop",
                    Title = "Publish products",
                    IconClass = "far fa-dot-circle",
                    Visible = true,
                    ControllerName = "PublishProducts",
                    ActionName = "Index",
                    RouteValues = new RouteValueDictionary() { { "area", "Admin" } },
                });


            //Insert Admin Config menu item for Omnishop Plugin
            var config = rootNode.ChildNodes.FirstOrDefault(node => node.SystemName.Equals("Configuration"));
            if (config == null)
                return;
            var plugins = config.ChildNodes.FirstOrDefault(node => node.SystemName.Equals("Local plugins"));
            if (plugins == null)
                return;

            var index = config.ChildNodes.IndexOf(plugins);
            if (index < 0)
                return;

            config.ChildNodes.Insert(index, new SiteMapNode
            {
                SystemName = "Omnishop Plugin",
                Title = "Omnishop",
                IconClass = "far fa-dot-circle",
                Visible = true,
                ControllerName = "OmnishopPluginConfiguration",
                ActionName = "Configure",
                RouteValues = new RouteValueDictionary { { "area", AreaNames.Admin } }
            });
        }

        /// <summary>
        /// Uninstall the plugin
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task UninstallAsync()
        {
            await base.UninstallAsync();
        }

    }
}
