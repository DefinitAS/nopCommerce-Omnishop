using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Misc.Omnishop.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Misc.Omnishop.Controllers
{
    [AutoValidateAntiforgeryToken]
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    public partial class OmnishopPluginConfigurationController : BasePluginController
    {
        private readonly IPermissionService _permissionService;
        private readonly IStoreContext _storeContext;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;

        public OmnishopPluginConfigurationController(IPermissionService permissionService, IStoreContext storeContext, ISettingService settingService)
        {
            _permissionService = permissionService;
            _storeContext = storeContext;
            _settingService = settingService;
        }

        public virtual async Task<IActionResult> Configure()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            //load settings for active store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var pluginSettings = await _settingService.LoadSettingAsync<OmnishopPluginSettings>(storeId);
            var model = new ConfigurationModel()
            {
                ActiveStoreScopeConfiguration = storeId,
                ForceCanonicalUrl = pluginSettings.ForceCanonicalUrl,
                ProductImportApiKey=pluginSettings.ProductImportApiKey
            };

            return View("~/Plugins/Misc.Omnishop/Views/Configure.cshtml", model);
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        [HttpPost, ActionName("Configure")]
        [FormValueRequired("save")]
        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
                return await Configure();

            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var pluginSettings = await _settingService.LoadSettingAsync<OmnishopPluginSettings>(storeId);

            pluginSettings.ForceCanonicalUrl = model.ForceCanonicalUrl;
            pluginSettings.ProductImportApiKey = model.ProductImportApiKey;
            await _settingService.SaveSettingAsync(pluginSettings);
            await _settingService.ClearCacheAsync();

            //_notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return await Configure();
        }
    }
}
