using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Nop.Core;
using Nop.Services.Common;
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

        public OmnishopPlugin(IPermissionService permissionService,
            IWebHelper webHelper)
        {
            _permissionService = permissionService;
            _webHelper = webHelper;
        }

        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/OmnishopAdmin/Configure";
        }

        public override async Task InstallAsync()
        {            
            await base.InstallAsync();
        }

        public async Task ManageSiteMapAsync(SiteMapNode rootNode)
        {
            //if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
            //    return;

            //var config = rootNode.ChildNodes.FirstOrDefault(node => node.SystemName.Equals("Configuration"));
            //if (config == null)
            //    return;

            //var plugins = config.ChildNodes.FirstOrDefault(node => node.SystemName.Equals("Local plugins"));

            //if (plugins == null)
            //    return;

            //var index = config.ChildNodes.IndexOf(plugins);

            //if (index < 0)
            //    return;

            //config.ChildNodes.Insert(index, new SiteMapNode
            //{
            //    SystemName = "Definit Simple Checkout",
            //    Title = "Simple Checkout",
            //    ControllerName = "OmnishopAdmin",
            //    ActionName = "Configure",
            //    IconClass = "far fa-dot-circle",
            //    Visible = true,
            //    RouteValues = new RouteValueDictionary { { "area", AreaNames.Admin } }
            //});
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
