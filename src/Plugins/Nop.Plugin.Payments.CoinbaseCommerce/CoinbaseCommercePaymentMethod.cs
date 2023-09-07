using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coinbase.Commerce;
using Coinbase.Commerce.Models;
using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Plugins;
using Nop.Core.Http.Extensions;
using Newtonsoft.Json;

namespace Nop.Plugin.Payments.CoinbaseCommerce
{

    public class CoinbaseCommercePaymentMethod : BasePlugin, IPaymentMethod
    {
        private readonly IAddressService _addressService;
        private readonly ICheckoutAttributeParser _checkoutAttributeParser;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILocalizationService _localizationService;
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;
        private readonly CoinbaseCommerceSettings _coinbaseCommerceSettings;

        public CoinbaseCommercePaymentMethod(IAddressService addressService,
            ICheckoutAttributeParser checkoutAttributeParser,
            IHttpContextAccessor httpContextAccessor,
            ILocalizationService localizationService,
            IOrderService orderService,
            IOrderTotalCalculationService orderTotalCalculationService,
            IProductService productService,
            ISettingService settingService,
            IWebHelper webHelper,
            CoinbaseCommerceSettings coinbaseCommerceSettings)
        {
            _addressService = addressService;
            _checkoutAttributeParser = checkoutAttributeParser;
            _httpContextAccessor = httpContextAccessor;
            _localizationService = localizationService;
            _orderService = orderService;
            _productService = productService;
            _settingService = settingService;
            _webHelper = webHelper;
            _coinbaseCommerceSettings = coinbaseCommerceSettings;
        }

        public async Task<ProcessPaymentResult> ProcessPaymentAsync(ProcessPaymentRequest processPaymentRequest)
        {
            _httpContextAccessor.HttpContext.Session.Remove("CoinbaseCommerceChargeResponse");
            var processPaymentResult = new ProcessPaymentResult
            {
                AllowStoringCreditCardNumber = false,
            };

            var commerceApi = new CommerceApi(_coinbaseCommerceSettings.ApiKey);
            var charge = new CreateChargeEx
            {
                Name = "Kjøp",
                Description = "Varekjøp fra plantedamp.no",
                PricingType = PricingType.FixedPrice,
                RedirectUrl = $"{_webHelper.GetStoreLocation()}Plugins/PaymentCoinbaseCommerce/PaymentReturn/{processPaymentRequest.OrderGuid}",
                CancelUrl = $"{_webHelper.GetStoreLocation()}Plugins/PaymentCoinbaseCommerce/PaymentCancel/{processPaymentRequest.OrderGuid}",

                //LocalPrice = new Money { Amount = processPaymentRequest.OrderTotal, Currency = "NOK" },
                LocalPrice = new Money { Amount = 30.0m, Currency = "NOK" },
                // You can put any custom info here but keep it minimal.
                Metadata =
                {
                    {"customerId", processPaymentRequest.CustomerId},
                    {"orderGuid", processPaymentRequest.OrderGuid},
                },
            };

            var chargeResponse = await commerceApi.CreateChargeAsync(charge);
            if (chargeResponse.HasWarnings())
            {
                foreach(var warn in chargeResponse.Warnings)
                {
                    processPaymentResult.AddError(warn);
                }
            }

            if (chargeResponse.HasError())
            {
                //throw new Exception(response.Error.Type + "\n" + response.Error.Message);
                processPaymentResult.AddError(chargeResponse.Error.Type + "\n" + chargeResponse.Error.Message);
                processPaymentResult.NewPaymentStatus = Core.Domain.Payments.PaymentStatus.Voided;
                return processPaymentResult;
            }
            _httpContextAccessor.HttpContext.Session.Set("CoinbaseCommerceChargeResponse", chargeResponse);

            processPaymentResult.AuthorizationTransactionId = chargeResponse.Data.Code;
            return processPaymentResult;
        }

        /// <summary>
        /// Post process payment (used by payment gateways that require redirecting to a third-party URL)
        /// </summary>
        /// <param name="postProcessPaymentRequest">Payment info required for an order processing</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public Task PostProcessPaymentAsync(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            var apiResponse= this._httpContextAccessor.HttpContext.Session.Get<Response<Charge>>("CoinbaseCommerceChargeResponse");
            _httpContextAccessor.HttpContext.Session.Remove("CoinbaseCommerceChargeResponse");
            _httpContextAccessor.HttpContext.Response.Redirect(apiResponse.Data.HostedUrl);
            return Task.CompletedTask;
        }


        /// <summary>
        /// Returns a value indicating whether payment method should be hidden during checkout
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the rue - hide; false - display.
        /// </returns>
        public Task<bool> HidePaymentMethodAsync(IList<ShoppingCartItem> cart)
        {
            //you can put any logic here
            //for example, hide this payment method if all products in the cart are downloadable
            //or hide this payment method if current customer is from certain country
            return Task.FromResult(false);
        }

        public Task<decimal> GetAdditionalHandlingFeeAsync(IList<ShoppingCartItem> cart)
        {
            return Task.FromResult(decimal.Zero);
        }

        /// <summary>
        /// Captures payment
        /// </summary>
        /// <param name="capturePaymentRequest">Capture payment request</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the capture payment result
        /// </returns>
        public async Task<CapturePaymentResult> CaptureAsync(CapturePaymentRequest capturePaymentRequest)
        {
            var commerceApi = new CommerceApi(_coinbaseCommerceSettings.ApiKey);
            var chargeResponse = await commerceApi.GetChargeAsync(capturePaymentRequest.Order.AuthorizationTransactionId);

            var lastStatus = chargeResponse.Data.Timeline.OrderByDescending(x => x.Time).First();
            if (lastStatus.Status=="COMPLETED")
            {
                return new CapturePaymentResult { NewPaymentStatus = Core.Domain.Payments.PaymentStatus.Paid };
            }

            return new CapturePaymentResult { Errors = new[] { "Payment is not completed. Last status: " + lastStatus.Status } };
        }

        /// <summary>
        /// Refunds a payment
        /// </summary>
        /// <param name="refundPaymentRequest">Request</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public Task<RefundPaymentResult> RefundAsync(RefundPaymentRequest refundPaymentRequest)
        {
            return Task.FromResult(new RefundPaymentResult { Errors = new[] { "Refund method not supported" } });
        }

        /// <summary>
        /// Voids a payment
        /// </summary>
        /// <param name="voidPaymentRequest">Request</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public Task<VoidPaymentResult> VoidAsync(VoidPaymentRequest voidPaymentRequest)
        {
            return Task.FromResult(new VoidPaymentResult { Errors = new[] { "Void method not supported" } });
        }

        /// <summary>
        /// Process recurring payment
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the process payment result
        /// </returns>
        public Task<ProcessPaymentResult> ProcessRecurringPaymentAsync(ProcessPaymentRequest processPaymentRequest)
        {
            return Task.FromResult(new ProcessPaymentResult { Errors = new[] { "Recurring payment not supported" } });
        }

        /// <summary>
        /// Cancels a recurring payment
        /// </summary>
        /// <param name="cancelPaymentRequest">Request</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public Task<CancelRecurringPaymentResult> CancelRecurringPaymentAsync(CancelRecurringPaymentRequest cancelPaymentRequest)
        {
            //always success
            return Task.FromResult(new CancelRecurringPaymentResult());
        }

        /// <summary>
        /// Gets a value indicating whether customers can complete a payment after order is placed but not completed (for redirection payment methods)
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public Task<bool> CanRePostProcessPaymentAsync(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            //it's not a redirection payment method. So we always return false
            return Task.FromResult(false);
        }

        public Task<IList<string>> ValidatePaymentFormAsync(IFormCollection form)
        {
            var warnings = new List<string>();
            return Task.FromResult<IList<string>>(warnings);
        }

        public Task<ProcessPaymentRequest> GetPaymentInfoAsync(IFormCollection form)
        {
            return Task.FromResult(new ProcessPaymentRequest
            {
            });
        }

        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/PaymentCoinbaseCommerce/Configure";
        }

        public string GetPublicViewComponentName()
        {
            return "PaymentCoinbaseCommerce";
        }

        public override async Task InstallAsync()
        {
            //settings
            var settings = new CoinbaseCommerceSettings
            {
            };
            await _settingService.SaveSettingAsync(settings);

            //locales
            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                ["Plugins.Payments.CoinbaseCommerce.Instructions"] = "Coinbase Commerce payment",
                ["Plugins.Payments.CoinbaseCommerce.PaymentMethodDescription"]="Betal med kryptovaluta gjennom Coinbase Commerce",
                ["Plugins.Payments.CoinbaseCommerce.Fields.ApiKey"] = "Api key",
                ["Plugins.Payments.CoinbaseCommerce.Fields.ApiKey.Hint"] = "Enter API key from Coinbase.",
                ["Plugins.Payments.CoinbaseCommerce.Fields.SharedSecretKey"] = "Shared secret key",
                ["Plugins.Payments.CoinbaseCommerce.Fields.SharedSecretKey.Hint"] = "Create as a sharec secret key that Coinbase webhooks must include in URL.",
            });

            await base.InstallAsync();
        }

        /// <summary>
        /// Uninstall the plugin
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task UninstallAsync()
        {
            //settings
            await _settingService.DeleteSettingAsync<CoinbaseCommerceSettings>();

            //locales
            await _localizationService.DeleteLocaleResourcesAsync("Plugins.Payments.CoinbaseCommerce");

            await base.UninstallAsync();
        }

        /// <summary>
        /// Gets a payment method description that will be displayed on checkout pages in the public store
        /// </summary>
        /// <remarks>
        /// return description of this payment method to be display on "payment method" checkout step. good practice is to make it localizable
        /// for example, for a redirection payment method, description may be like this: "You will be redirected to PayPal site to complete the payment"
        /// </remarks>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task<string> GetPaymentMethodDescriptionAsync()
        {
            return await _localizationService.GetResourceAsync("Plugins.Payments.CoinbaseCommerce.PaymentMethodDescription");
        }


        public bool SupportCapture => true;
        public bool SupportPartiallyRefund => false;
        public bool SupportRefund => false;
        public bool SupportVoid => false;
        public RecurringPaymentType RecurringPaymentType => RecurringPaymentType.NotSupported;
        public PaymentMethodType PaymentMethodType => PaymentMethodType.Redirection;
        public bool SkipPaymentInfo => true;

    }
    public class CreateChargeEx : CreateCharge
    {
        /// <summary>
        /// Hosted charge URL
        /// </summary>
        [JsonProperty("cancel_url")]
        public string CancelUrl { get; set; }

    }
}

