using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Http;
using Nop.Plugin.Misc.Omnishop.Models;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Orders;
using OmnishopConnector.Model;
using OmnishopConnector.Model.SqlTypes;

namespace Nop.Plugin.Misc.Omnishop.Services
{
    public class OmnishopOrderService : IOmnishopOrderService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILocalizationService _localizationService;
        private readonly IProductService _productService;
        private readonly IAddressService _addressService;
        private readonly ICustomerService _customerService;
        private readonly IOrderService _orderService;
        private readonly IStoreContext _storeContext;
        private readonly ILogger _logger;
        private readonly ISettingService _settingService;

        public OmnishopOrderService(    IHttpClientFactory httpClientFactory,
                                        ILocalizationService localizationService,
                                        IProductService productService,
                                        IAddressService addressService,
                                        ICustomerService customerService,
                                        IOrderService orderService,
                                        IStoreContext storeContext,
                                        ILogger logger,
                                        ISettingService settingService)
        {
            _httpClientFactory = httpClientFactory;
            _localizationService = localizationService;
            _productService = productService;
            _addressService = addressService;
            _customerService = customerService;
            _orderService = orderService;
            _storeContext = storeContext;
            _logger = logger;
            _settingService = settingService;
        }
        public async Task SubmitOrderToOmnishop(Order order)
        {
            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var pluginSettings = await _settingService.LoadSettingAsync<OmnishopPluginSettings>(storeId);

            var omniOrder = await nopOrderToOmnishopOrder(order, pluginSettings);


            var httpClient = _httpClientFactory.CreateClient(NopHttpDefaults.DefaultHttpClient);
            httpClient.BaseAddress = new Uri(pluginSettings.ApiUrlBase);
            var requestContent = JsonContent.Create(omniOrder);
            var responseMessage = await httpClient.PostAsync($"/pos/{pluginSettings.DefaultClientId}/api/orders/submit", requestContent);
            responseMessage.EnsureSuccessStatusCode();
            var responseContent = await responseMessage.Content.ReadFromJsonAsync<dynamic>();

            order.OrderStatusId = 25;
            await _orderService.UpdateOrderAsync(order);
        }

        private async Task<tblOrder> nopOrderToOmnishopOrder(Order nopOrder, OmnishopPluginSettings pluginSettings)
        {
            var nopShippingAddress = await _addressService.GetAddressByIdAsync(nopOrder.ShippingAddressId ?? nopOrder.BillingAddressId);
            var nopCustomer = await _customerService.GetCustomerByIdAsync(nopOrder.CustomerId);
            var nopOrderItems = await _orderService.GetOrderItemsAsync(nopOrder.Id);

            var omniCustomer = new tblCustomer()
            {
                ExternalId = nopOrder.CustomerId.ToString(),
                ExternalOrigin = pluginSettings.OriginCode,
                Email = string.IsNullOrEmpty(nopCustomer.Email) ? nopShippingAddress.Email : nopCustomer.Email,
                Name = GetFullName(nopShippingAddress.FirstName, nopShippingAddress.LastName),
                Address = GetFullAddress(nopShippingAddress.Address1, nopShippingAddress.Address2),
                PostalCodeId = nopShippingAddress.ZipPostalCode,
                Phone = nopShippingAddress.PhoneNumber,
            };

            var omniOrder = new OmnishopConnector.Model.tblOrder()
            {
                CustomerAddress = omniCustomer.Address,
                CustomerEmail = omniCustomer.Email,
                CustomerName = omniCustomer.Name,
                CustomerPhone = omniCustomer.Phone,
                CustomerPostalCodeId = omniCustomer.PostalCodeId,
                navCustomer = omniCustomer,
                EmployeeId = BKeyChar16.FromString(pluginSettings.EmployeeId),
                ExternalId = nopOrder.Id.ToString(),
                ExternalOrigin = pluginSettings.OriginCode,
                Status = OmnishopConnector.Enums.OrderStatusCodes.Created,
                ExternalPaymentMethod = nopOrder.PaymentMethodSystemName,
                ExternalShippingMethod = nopOrder.ShippingMethod,

                navOrderDetails = new List<tblOrderDetail>(),
                navOrderTextLines = new List<tblOrderTextLine>(),
            };

            int iLineNo = 1;
            foreach (var orderItem in nopOrderItems)
            {
                var product = await _productService.GetProductByIdAsync(orderItem.ProductId);
                var omniOrderDetail = new tblOrderDetail()
                {
                    ProductId = BKeyChar16.FromString(product.Sku),
                    ProductName = product.Name,
                    VatRateId = product.TaxCategoryId,
                    Quantity = orderItem.Quantity,
                    GrossPriceExVat = orderItem.UnitPriceExclTax,
                    NetPriceExVat = orderItem.UnitPriceExclTax,
                    LineNo = iLineNo++,
                };
                omniOrder.navOrderDetails.Add(omniOrderDetail);
            }

            var shippingCost = (decimal)nopOrder.OrderShippingExclTax;
            if (shippingCost != 0)
            {
                omniOrder.navOrderDetails.Add(new tblOrderDetail()
                {
                    ProductId = BKeyChar16.FromString(pluginSettings.ProductIdShipping),
                    ProductName = nopOrder.ShippingMethod,
                    VatRateId = 1,
                    Quantity = 1,
                    GrossPriceExVat = shippingCost,
                    NetPriceExVat = shippingCost,
                    LineNo = iLineNo++,
                });
            }

            return omniOrder;
        }

        private static string GetFullName(string firstName, string lastName)
        {
            return firstName + " " + lastName;
        }

        private static string GetFullAddress(string address1, string address2)
        {
            if (string.IsNullOrEmpty(address1))
                return address2;

            if (string.IsNullOrEmpty(address2))
                return address1;

            return address1 + "\n" + address2;
        }
    }
}
