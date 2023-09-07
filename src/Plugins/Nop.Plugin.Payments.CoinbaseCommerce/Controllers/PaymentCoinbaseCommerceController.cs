using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Coinbase.Commerce;
using Coinbase.Commerce.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Plugin.Payments.CoinbaseCommerce.Models;
using Nop.Services;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Payments.CoinbaseCommerce.Controllers
{

    [AutoValidateAntiforgeryToken]
    public class PaymentCoinbaseCommerceController : BasePaymentController
    {
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IOrderService _orderService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly CoinbaseCommerceSettings _coinbaseCommerceSettings;


        public PaymentCoinbaseCommerceController(ILocalizationService localizationService,
            INotificationService notificationService,
            IOrderService orderService,
            IOrderProcessingService orderProcessingService,
            IPermissionService permissionService,
            ISettingService settingService,
            IStoreContext storeContext, 
            CoinbaseCommerceSettings coinbaseCommerceSettings)
        {
            _localizationService = localizationService;
            _notificationService = notificationService;
            _orderService = orderService;
            _orderProcessingService = orderProcessingService;
            _permissionService = permissionService;
            _settingService = settingService;
            _storeContext = storeContext;
            _coinbaseCommerceSettings = coinbaseCommerceSettings;

        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public async Task<IActionResult> Configure()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var manualPaymentSettings = await _settingService.LoadSettingAsync<CoinbaseCommerceSettings>(storeScope);
            var model = new ConfigurationModel
            {
                ActiveStoreScopeConfiguration = storeScope,
                ApiKey = manualPaymentSettings.ApiKey,
                SharedSecretKey = manualPaymentSettings.SharedSecretKey,
            };
            if (storeScope > 0)
            {
                model.ApiKey_OverrideForStore = await _settingService.SettingExistsAsync(manualPaymentSettings, x => x.ApiKey, storeScope);
                model.SharedSecretKey_OverrideForStore = await _settingService.SettingExistsAsync(manualPaymentSettings, x => x.SharedSecretKey, storeScope);
            }

            return View("~/Plugins/Payments.CoinbaseCommerce/Views/Configure.cshtml", model);
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
            var manualPaymentSettings = await _settingService.LoadSettingAsync<CoinbaseCommerceSettings>(storeScope);

            //save settings
            manualPaymentSettings.ApiKey = model.ApiKey;
            manualPaymentSettings.SharedSecretKey = model.SharedSecretKey;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            await _settingService.SaveSettingOverridablePerStoreAsync(manualPaymentSettings, x => x.ApiKey, model.ApiKey_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(manualPaymentSettings, x => x.SharedSecretKey, model.SharedSecretKey_OverrideForStore, storeScope, false);

            //now clear settings cache
            await _settingService.ClearCacheAsync();

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return await Configure();
        }

        [HttpGet]
        //Coinbase paymentpage redirects to this when user has completed payment
        public async Task<IActionResult> PaymentReturn(string orderGuid)
        {
            var order = await _orderService.GetOrderByGuidAsync(Guid.Parse(orderGuid));
            var commerceApi = new CommerceApi(_coinbaseCommerceSettings.ApiKey);
            var chargeResponse = await commerceApi.GetChargeAsync(order.AuthorizationTransactionId);

            var lastStatus = chargeResponse.Data.Timeline.OrderByDescending(x => x.Time).First();
            switch (lastStatus.Status)
            {
                case "NEW":
                    order.PaymentStatus = Core.Domain.Payments.PaymentStatus.Pending;
                    return await OrderCompleted(order);
                case "PENDING":
                    //Transaction has been initiated (confirmed on blockchain)
                    order.PaymentStatus = Core.Domain.Payments.PaymentStatus.Authorized;
                    return await OrderCompleted(order);
                case "COMPLETED":
                    order.PaymentStatus = Core.Domain.Payments.PaymentStatus.Paid;
                    order.PaidDateUtc = lastStatus.Time.DateTime;
                    return await OrderCompleted(order);
                case "EXPIRED":
                    //order.PaymentStatus = Core.Domain.Payments.PaymentStatus.Paid;
                    break;
                case "UNRESOLVED":
                    //order.PaymentStatus = Core.Domain.Payments.PaymentStatus.Paid;
                    break;
                case "RESOLVED":
                    //order.PaymentStatus = Core.Domain.Payments.PaymentStatus.Paid;
                    break;
                case "CANCELED":
                    return await PaymentCancelCore(order);
                default:
                    //order.PaymentStatus = Core.Domain.Payments.PaymentStatus.Paid;
                    break;
            }

            return await OrderCompleted(order);
        }

        //Coinbase paymentpage redirects to this if user cancels payment
        [HttpGet]
        public async Task<IActionResult> PaymentCancel(string orderGuid)
        {
            var order = await _orderService.GetOrderByGuidAsync(Guid.Parse(orderGuid));
            return await PaymentCancelCore(order);
        }

        private async Task<IActionResult> OrderCompleted(Core.Domain.Orders.Order order)
        {
            await _orderService.UpdateOrderAsync(order);
            await _orderProcessingService.CheckOrderStatusAsync(order);
            //_commonService.SendPlacedOrderEmailMessage(order);
            return RedirectToRoute("CheckoutCompleted", new { orderId = order.Id });
        }

        private async Task<IActionResult> PaymentCancelCore(Core.Domain.Orders.Order order)
        {
            var commerceApi = new CommerceApi(_coinbaseCommerceSettings.ApiKey);

            var chargeResponse = await commerceApi.GetChargeAsync(order.AuthorizationTransactionId);
            //TODO: Ikke slett hvis chargeresponse viser at betaling er initialisert?

            await _orderProcessingService.ReOrderAsync(order);
            await _orderProcessingService.DeleteOrderAsync(order);
            return RedirectToRoute("ShoppingCart");
        }

        [HttpPost]
        public async Task<ActionResult> PaymentWebhook()
        {
            var SHARED_SECRET = "";
            var requestSignature = Request.Headers[HeaderNames.WebhookSignature];
            var requestJson = await GetRawBodyAsync(Request);
            //Request.InputStream.Seek(0, SeekOrigin.Begin);
            //var json = new StreamReader(Request.InputStream).ReadToEnd();

            if (!WebhookHelper.IsValid(SHARED_SECRET, requestSignature, requestJson))
            {
                return BadRequest();
            }

            var webhook = JsonConvert.DeserializeObject<Webhook>(requestJson);
            if (webhook.Event.IsChargeConfirmed)
            {
                var charge = webhook.Event.DataAs<Charge>();

                if (charge.Name == "PRODUCT_NAME")
                {
                    //THE PAYMENT IS SUCCESSFUL
                    //DO SOMETHING TO MARK THE PAYMENT IS COMPLETE
                    //IN YOUR DATABASE
                }
            }
            return Ok();

        }

        private static async Task<string> GetRawBodyAsync(HttpRequest request, Encoding encoding = null)      
        {
            if (!request.Body.CanSeek)
            {
                // We only do this if the stream isn't *already* seekable,
                // as EnableBuffering will create a new stream instance
                // each time it's called
                request.EnableBuffering();
            }

            request.Body.Position = 0;
            var reader = new StreamReader(request.Body, encoding ?? Encoding.UTF8);
            var body = await reader.ReadToEndAsync().ConfigureAwait(false);
            request.Body.Position = 0;
            return body;
        }
    }
}