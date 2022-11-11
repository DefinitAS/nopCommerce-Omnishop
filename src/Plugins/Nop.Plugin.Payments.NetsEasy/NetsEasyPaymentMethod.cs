using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Plugins.Payments.NetsEasy.Services;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Plugins;
using Vendr.Contrib.PaymentProviders.Api.Models;

namespace Nop.Plugin.Payments.NetsEasy
{

    public class NetsEasyPaymentMethod : BasePlugin, IPaymentMethod
    {
        private readonly IAddressService _addressService;
        private readonly ICheckoutAttributeParser _checkoutAttributeParser;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILocalizationService _localizationService;
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;
        private readonly NetsEasyHttpClient _netsEasyHttpClient;
        private readonly NetsEasySettings _netsEasySettings;

        public NetsEasyPaymentMethod(IAddressService addressService,
            ICheckoutAttributeParser checkoutAttributeParser,
            IHttpContextAccessor httpContextAccessor,
            ILocalizationService localizationService,
            IOrderService orderService,
            IOrderTotalCalculationService orderTotalCalculationService,
            IProductService productService,
            ISettingService settingService,
            IWebHelper webHelper,
            NetsEasyHttpClient netsEasyHttpClient,
            NetsEasySettings manualPaymentSettings)
        {
            _addressService = addressService;
            _checkoutAttributeParser = checkoutAttributeParser;
            _httpContextAccessor = httpContextAccessor;
            _localizationService = localizationService;
            _orderService = orderService;
            _productService = productService;
            _settingService = settingService;
            _webHelper = webHelper;
            _netsEasyHttpClient = netsEasyHttpClient;
            _netsEasySettings = manualPaymentSettings;
        }

        public Task<ProcessPaymentResult> ProcessPaymentAsync(ProcessPaymentRequest processPaymentRequest)
        {
            var result = new ProcessPaymentResult
            {
                
            };
            return Task.FromResult(result);
        }

        /// <summary>
        /// Post process payment (used by payment gateways that require redirecting to a third-party URL)
        /// </summary>
        /// <param name="postProcessPaymentRequest">Payment info required for an order processing</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task PostProcessPaymentAsync(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            var paymentModel = await CreateNetsPaymentModel(postProcessPaymentRequest);
            try
            {
                var paymentResult = await _netsEasyHttpClient.CreatePaymentAsync(paymentModel);
                postProcessPaymentRequest.Order.AuthorizationTransactionId = paymentResult.PaymentId;
                await _orderService.UpdateOrderAsync(postProcessPaymentRequest.Order);
                _httpContextAccessor.HttpContext.Response.Redirect(paymentResult.HostedPaymentPageUrl);
            }
            catch (Exception)
            {
                throw;
                //_orderProcessingService.CancelOrder(postProcessPaymentRequest.Order, false);
                //postProcessPaymentRequest.Order.PaymentStatus = Core.Domain.Payments.PaymentStatus.Voided;
                //_orderService.UpdateOrder(postProcessPaymentRequest.Order);
                //_httpContextAccessor.HttpContext.Response.Redirect($"{_webHelper.GetStoreLocation()}Plugins/NetsEasy/FallbackAction/{postProcessPaymentRequest.Order.Id.ToString()}");
            }
        }

        private async Task<NetsPaymentRequest> CreateNetsPaymentModel(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            var billingAddress = await _addressService.GetAddressByIdAsync(postProcessPaymentRequest.Order.BillingAddressId);

            var paymentModel = new NetsPaymentRequest()
            {
                Order = new NetsOrder()
                {
                    Amount = Convert.ToInt32(postProcessPaymentRequest.Order.OrderTotal * 100m),
                    Currency = "NOK",
                    Reference = postProcessPaymentRequest.Order.Id.ToString(),
                    Items = new List<NetsOrderItem>()
                },
                Checkout = new NetsCheckout()
                {
                    Charge = false,
                    PublicDevice = false,
                    //"EmbeddedCheckout" - Payment is embedded in IFrame in a page on our site. Url specifies the url on our site where the IFrame is hosted.
                    IntegrationType = "HostedPaymentPage",
                    Url = string.Empty,
                    ReturnUrl = $"{_webHelper.GetStoreLocation()}Plugins/PaymentNetsEasy/PaymentReturn/{postProcessPaymentRequest.Order.Id}",
                    CancelUrl = $"{_webHelper.GetStoreLocation()}Plugins/PaymentNetsEasy/PaymentCancel/{postProcessPaymentRequest.Order.Id}",
                    TermsUrl = $"{_webHelper.GetStoreLocation()}conditions-of-use",
                    MerchantHandlesConsumerData = true,
                    CountryCode ="NOR",
                    Consumer = new NetsConsumer()
                    {
                        Email = billingAddress.Email,
                        ShippingAddress = new NetsAddress()
                        {
                            Line1 = billingAddress.Address1,
                            Line2 = billingAddress.Address2 ?? string.Empty,
                            PostalCode = billingAddress.ZipPostalCode,
                            City = billingAddress.City,
                            Country = "NOR"
                        },
                        PhoneNumber = new NetsCustomerPhone()
                        {
                            Prefix = "+47",
                            Number = billingAddress.PhoneNumber.Replace("+47", "")
                        }
                    },
                    //ConsumerType = new ConsumerType()
                    //{
                    //    Default = "B2C",
                    //    SupportedTypes = new List<string>()
                    //    {
                    //        "B2C"
                    //    }
                    //},
                    //MerchantHandlesShippingCost = false,
                    //Shipping = new Shipping()
                    //{
                    //    Countries = new List<Country>()
                    //    {
                    //        new Country()
                    //        {
                    //            CountryCode = "NOR"
                    //        }
                    //    }
                    //}
                }
            };

            var checkoutAttributeValues = _checkoutAttributeParser.ParseCheckoutAttributeValues(postProcessPaymentRequest.Order.CheckoutAttributesXml);
            if (checkoutAttributeValues != null)
            {
                await foreach (var (attribute, values) in checkoutAttributeValues)
                {
                    await foreach (var attributeValue in values)
                    {
                        if (attributeValue.PriceAdjustment != decimal.Zero)
                        {
                            paymentModel.Order.Items.Add(new NetsOrderItem()
                            {
                                Reference = attributeValue.CheckoutAttributeId.ToString(),
                                Name = attributeValue.Name,
                                Quantity = 1,
                                Unit = "pcs",
                                TaxRate = Convert.ToInt32((attributeValue.PriceAdjustment - attributeValue.PriceAdjustment) / attributeValue.PriceAdjustment * 10000m),
                                UnitPrice = Convert.ToInt32(attributeValue.PriceAdjustment * 100m),
                                TaxAmount = Convert.ToInt32((attributeValue.PriceAdjustment - attributeValue.PriceAdjustment) * 100m),
                                GrossTotalAmount = Convert.ToInt32(attributeValue.PriceAdjustment * 100m),
                                NetTotalAmount = Convert.ToInt32(attributeValue.PriceAdjustment * 100m)
                            });
                        }
                    }
                }
            }

            if (postProcessPaymentRequest.Order.OrderShippingExclTax != decimal.Zero && postProcessPaymentRequest.Order.OrderShippingInclTax != decimal.Zero)
            {
                paymentModel.Order.Items.Add(new NetsOrderItem()
                {
                    Name = postProcessPaymentRequest.Order.ShippingMethod,
                    Quantity = 1,
                    Reference = postProcessPaymentRequest.Order.ShippingMethod + "_" + postProcessPaymentRequest.Order.Id.ToString(),
                    Unit = "pcs",
                    UnitPrice = Convert.ToInt32(postProcessPaymentRequest.Order.OrderShippingExclTax * 100m),
                    TaxRate = Convert.ToInt32((postProcessPaymentRequest.Order.OrderShippingInclTax - postProcessPaymentRequest.Order.OrderShippingExclTax) / postProcessPaymentRequest.Order.OrderShippingExclTax * 10000m),
                    TaxAmount = Convert.ToInt32((postProcessPaymentRequest.Order.OrderShippingInclTax - postProcessPaymentRequest.Order.OrderShippingExclTax) * 100m),
                    GrossTotalAmount = Convert.ToInt32(postProcessPaymentRequest.Order.OrderShippingInclTax * 100m),
                    NetTotalAmount = Convert.ToInt32(postProcessPaymentRequest.Order.OrderShippingExclTax * 100m)
                });
            }

            if (postProcessPaymentRequest.Order.PaymentMethodAdditionalFeeExclTax != decimal.Zero && postProcessPaymentRequest.Order.PaymentMethodAdditionalFeeInclTax != decimal.Zero)
            {
                paymentModel.Order.Items.Add(new NetsOrderItem()
                {
                    Name = postProcessPaymentRequest.Order.PaymentMethodSystemName,
                    Quantity = 1,
                    Reference = postProcessPaymentRequest.Order.PaymentMethodSystemName + "_" + postProcessPaymentRequest.Order.Id.ToString(),
                    Unit = "pcs",
                    UnitPrice = Convert.ToInt32(postProcessPaymentRequest.Order.PaymentMethodAdditionalFeeExclTax * 100m),
                    TaxRate = Convert.ToInt32((postProcessPaymentRequest.Order.PaymentMethodAdditionalFeeInclTax - postProcessPaymentRequest.Order.PaymentMethodAdditionalFeeExclTax) / postProcessPaymentRequest.Order.PaymentMethodAdditionalFeeExclTax * 10000m),
                    TaxAmount = Convert.ToInt32((postProcessPaymentRequest.Order.PaymentMethodAdditionalFeeInclTax - postProcessPaymentRequest.Order.PaymentMethodAdditionalFeeExclTax) * 100m),
                    GrossTotalAmount = Convert.ToInt32(postProcessPaymentRequest.Order.PaymentMethodAdditionalFeeInclTax * 100m),
                    NetTotalAmount = Convert.ToInt32(postProcessPaymentRequest.Order.PaymentMethodAdditionalFeeExclTax * 100m)
                });
            }

            foreach (var item in await _orderService.GetOrderItemsAsync(postProcessPaymentRequest.Order.Id))
            {
                var product = await _productService.GetProductByIdAsync(item.ProductId);
                paymentModel.Order.Items.Add(new NetsOrderItem()
                {
                    Reference = product.Sku,
                    Name = product.Name,
                    Quantity = item.Quantity,
                    Unit = "pcs",
                    TaxRate = Convert.ToInt32((item.PriceInclTax - item.PriceExclTax) / item.PriceExclTax * 10000m),
                    UnitPrice = Convert.ToInt32(item.UnitPriceExclTax * 100m),
                    TaxAmount = Convert.ToInt32((item.PriceInclTax - item.PriceExclTax) * 100m),
                    GrossTotalAmount = Convert.ToInt32(item.PriceInclTax * 100m),
                    NetTotalAmount = Convert.ToInt32(item.PriceExclTax * 100m)
                });
            }

            paymentModel.Order.Amount = paymentModel.Order.Items.Sum(x => x.GrossTotalAmount);
            return paymentModel;
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
        public Task<CapturePaymentResult> CaptureAsync(CapturePaymentRequest capturePaymentRequest)
        {
            return Task.FromResult(new CapturePaymentResult { Errors = new[] { "Capture method not supported" } });
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
            return $"{_webHelper.GetStoreLocation()}Admin/PaymentNetsEasy/Configure";
        }

        public string GetPublicViewComponentName()
        {
            return "PaymentNetsEasy";
        }

        public override async Task InstallAsync()
        {
            //settings
            var settings = new NetsEasySettings
            {
                Url= "https://test.api.dibspayment.eu/"
            };
            await _settingService.SaveSettingAsync(settings);

            //locales
            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                //["Plugins.Payments.NetsEasy.Instructions"] = "This payment method stores credit card information in database (it's not sent to any third-party processor). In order to store credit card information, you must be PCI compliant.",
                //["Plugins.Payments.NetsEasy.Fields.AdditionalFee"] = "Additional fee",
                //["Plugins.Payments.NetsEasy.Fields.AdditionalFee.Hint"] = "Enter additional fee to charge your customers.",
                //["Plugins.Payments.NetsEasy.Fields.AdditionalFeePercentage"] = "Additional fee. Use percentage",
                //["Plugins.Payments.NetsEasy.Fields.AdditionalFeePercentage.Hint"] = "Determines whether to apply a percentage additional fee to the order total. If not enabled, a fixed value is used.",
                //["Plugins.Payments.NetsEasy.Fields.TransactMode"] = "After checkout mark payment as",
                //["Plugins.Payments.NetsEasy.Fields.TransactMode.Hint"] = "Specify transaction mode.",
                //["Plugins.Payments.NetsEasy.PaymentMethodDescription"] = "Pay by credit / debit card"
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
            await _settingService.DeleteSettingAsync<NetsEasySettings>();

            //locales
            await _localizationService.DeleteLocaleResourcesAsync("Plugins.Payments.NetsEasy");

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
            return await _localizationService.GetResourceAsync("Plugins.Payments.NetsEasy.PaymentMethodDescription");
        }


        public bool SupportCapture => true;
        public bool SupportPartiallyRefund => false;
        public bool SupportRefund => false;
        public bool SupportVoid => false;
        public RecurringPaymentType RecurringPaymentType => RecurringPaymentType.NotSupported;
        public PaymentMethodType PaymentMethodType => PaymentMethodType.Redirection;
        public bool SkipPaymentInfo => true;

    }
}