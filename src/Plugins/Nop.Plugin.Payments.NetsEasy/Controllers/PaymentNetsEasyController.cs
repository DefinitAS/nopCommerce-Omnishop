using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Payments.NetsEasy.Models;
using Nop.Plugins.Payments.NetsEasy.Services;
using Nop.Services;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Payments.NetsEasy.Controllers
{

    [AutoValidateAntiforgeryToken]
    public class PaymentNetsEasyController : BasePaymentController
    {
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IOrderService _orderService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly NetsEasyHttpClient _netsEasyHttpClient;
        private readonly IStoreContext _storeContext;

        public PaymentNetsEasyController(ILocalizationService localizationService,
            INotificationService notificationService,
            IOrderService orderService,
            IOrderProcessingService orderProcessingService,
            IPermissionService permissionService,
            ISettingService settingService,
            NetsEasyHttpClient netsEasyHttpClient,
            IStoreContext storeContext)
        {
            _localizationService = localizationService;
            _notificationService = notificationService;
            _orderService = orderService;
            _orderProcessingService = orderProcessingService;
            _permissionService = permissionService;
            _settingService = settingService;
            _netsEasyHttpClient = netsEasyHttpClient;
            _storeContext = storeContext;
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public async Task<IActionResult> Configure()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var manualPaymentSettings = await _settingService.LoadSettingAsync<NetsEasySettings>(storeScope);
            var model = new ConfigurationModel
            {
                ActiveStoreScopeConfiguration = storeScope,
                MerchantId = manualPaymentSettings.MerchantId,
                SecretKey = manualPaymentSettings.SecretKey,
                CheckoutKey = manualPaymentSettings.CheckoutKey,
                Url = manualPaymentSettings.Url
            };
            if (storeScope > 0)
            {
                model.MerchantId_OverrideForStore = await _settingService.SettingExistsAsync(manualPaymentSettings, x => x.MerchantId, storeScope);
                model.SecretKey_OverrideForStore = await _settingService.SettingExistsAsync(manualPaymentSettings, x => x.SecretKey, storeScope);
                model.CheckoutKey_OverrideForStore = await _settingService.SettingExistsAsync(manualPaymentSettings, x => x.CheckoutKey, storeScope);
                model.Url_OverrideForStore = await _settingService.SettingExistsAsync(manualPaymentSettings, x => x.Url, storeScope);
            }

            return View("~/Plugins/Payments.NetsEasy/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return await Configure();

            //load settings for a chosen store scope
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var manualPaymentSettings = await _settingService.LoadSettingAsync<NetsEasySettings>(storeScope);

            //save settings
            manualPaymentSettings.MerchantId = model.MerchantId;
            manualPaymentSettings.Url = model.Url;
            manualPaymentSettings.SecretKey = model.SecretKey;
            manualPaymentSettings.CheckoutKey = model.CheckoutKey;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */

            await _settingService.SaveSettingOverridablePerStoreAsync(manualPaymentSettings, x => x.MerchantId, model.MerchantId_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(manualPaymentSettings, x => x.Url, model.Url_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(manualPaymentSettings, x => x.SecretKey, model.SecretKey_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(manualPaymentSettings, x => x.CheckoutKey, model.CheckoutKey_OverrideForStore, storeScope, false);

            //now clear settings cache
            await _settingService.ClearCacheAsync();

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return await Configure();
        }

        [HttpGet]
        //Nets paymentpage redirects to this when user has completed payment
        public async Task<IActionResult> PaymentReturn(int orderId)
        {
            var order = await _orderService.GetOrderByIdAsync(orderId);
            var paymentResponse = await _netsEasyHttpClient.GetPaymentAsync(order.AuthorizationTransactionId);
            if(paymentResponse.Payment.Summary.ReservedAmount!=0)
            {
                order.PaymentStatus = Core.Domain.Payments.PaymentStatus.Authorized;
                await _orderService.UpdateOrderAsync(order);
                await _orderProcessingService.CheckOrderStatusAsync(order);
                //_commonService.SendPlacedOrderEmailMessage(order);
                return RedirectToRoute("CheckoutCompleted", new { orderId = order.Id });
            }

            await _orderProcessingService.ReOrderAsync(order);
            await _orderProcessingService.DeleteOrderAsync(order);
            return RedirectToRoute("ShoppingCart");
        }


        //Nets paymentpage redirects to this if user cancels payment
        [HttpGet]
        public async Task<IActionResult> PaymentCancel(int orderId)
        {
            var order = await _orderService.GetOrderByIdAsync(orderId);

            await _orderProcessingService.ReOrderAsync(order);
            await _orderProcessingService.DeleteOrderAsync(order);
            return RedirectToRoute("ShoppingCart");
        }
    }
}