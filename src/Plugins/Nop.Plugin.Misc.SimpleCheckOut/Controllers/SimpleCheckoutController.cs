using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Http.Extensions;
using Nop.Plugin.Misc.SimpleCheckOut.Models;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Web.Factories;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Misc.SimpleCheckOut.Controllers
{
    [AutoValidateAntiforgeryToken]
    public partial class SimpleCheckoutController : BasePluginController
    {
        private readonly AddressSettings _addressSettings;
        private readonly CustomerSettings _customerSettings;
        private readonly IAddressAttributeParser _addressAttributeParser;
        private readonly IAddressService _addressService;
        private readonly ICustomerModelFactory _customerModelFactory;
        private readonly ICheckoutModelFactory _checkoutModelFactory;
        private readonly ICountryService _countryService;
        private readonly ICustomerService _customerService;
        private readonly ICustomerRegistrationService _customerRegistrationService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IOrderService _orderService;
        private readonly IPaymentPluginManager _paymentPluginManager;
        private readonly IPaymentService _paymentService;
        private readonly IProductService _productService;
        private readonly IShippingService _shippingService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IStoreContext _storeContext;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;
        private readonly OrderSettings _orderSettings;
        private readonly PaymentSettings _paymentSettings;
        private readonly RewardPointsSettings _rewardPointsSettings;
        private readonly ShippingSettings _shippingSettings;

        public SimpleCheckoutController(
            IPermissionService permissionService,
            AddressSettings addressSettings,
            CustomerSettings customerSettings,
            IAddressAttributeParser addressAttributeParser,
            IAddressService addressService,
            ICustomerModelFactory customerModelFactory,
            ICheckoutModelFactory checkoutModelFactory,
            ICountryService countryService,
            ICustomerService customerService,
            ICustomerRegistrationService customerRegistrationService,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            ILogger logger,
            IOrderProcessingService orderProcessingService,
            IOrderService orderService,
            IPaymentPluginManager paymentPluginManager,
            IPaymentService paymentService,
            IProductService productService,
            IShippingService shippingService,
            IShoppingCartService shoppingCartService,
            IStoreContext storeContext,
            IWebHelper webHelper,
            IWorkContext workContext,
            OrderSettings orderSettings,
            PaymentSettings paymentSettings,
            RewardPointsSettings rewardPointsSettings,
            ShippingSettings shippingSettings)
        {
            _addressSettings = addressSettings;
            _customerSettings = customerSettings;
            _addressAttributeParser = addressAttributeParser;
            _addressService = addressService;
            _customerModelFactory = customerModelFactory;
            _checkoutModelFactory = checkoutModelFactory;
            _countryService = countryService;
            _customerService = customerService;
            _customerRegistrationService = customerRegistrationService;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _logger = logger;
            _orderProcessingService = orderProcessingService;
            _orderService = orderService;
            _paymentPluginManager = paymentPluginManager;
            _paymentService = paymentService;
            _productService = productService;
            _shippingService = shippingService;
            _shoppingCartService = shoppingCartService;
            _storeContext = storeContext;
            _webHelper = webHelper;
            _workContext = workContext;
            _orderSettings = orderSettings;
            _paymentSettings = paymentSettings;
            _rewardPointsSettings = rewardPointsSettings;
            _shippingSettings = shippingSettings;
        }

        public virtual async Task<IActionResult> Index()
        {
            var checkoutIndexResult = await ValidateCheckoutIsValid();
            if (checkoutIndexResult != null)
            {
                return checkoutIndexResult;
            }

            var customer = await _workContext.GetCurrentCustomerAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);
            var model = await CreateSimpleCheckoutModel(customer, cart);

            //ShippingMethod
            var shippingMethodModel = await _checkoutModelFactory.PrepareShippingMethodModelAsync(cart, await _customerService.GetCustomerShippingAddressAsync(customer));
            var selectedShippingOption = await _genericAttributeService.GetAttributeAsync<ShippingOption>(customer, NopCustomerDefaults.SelectedShippingOptionAttribute, store.Id);
            if (selectedShippingOption == null && shippingMethodModel.ShippingMethods.Any())
            {
                await _genericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.SelectedShippingOptionAttribute, shippingMethodModel.ShippingMethods.First().ShippingOption, store.Id);
            }

            //PaymentMethod
            var filterByCountryId = 0;
            if (_addressSettings.CountryEnabled)
            {
                filterByCountryId = (await _customerService.GetCustomerBillingAddressAsync(customer))?.CountryId ?? 0;
            }
            var paymentMethodModel = await _checkoutModelFactory.PreparePaymentMethodModelAsync(cart, filterByCountryId);
            var selectedPaymentOption = await _genericAttributeService.GetAttributeAsync<ShippingOption>(customer, NopCustomerDefaults.SelectedPaymentMethodAttribute, store.Id);
            if (selectedPaymentOption == null && paymentMethodModel.PaymentMethods.Any())
            {
                await _genericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.SelectedPaymentMethodAttribute, paymentMethodModel.PaymentMethods[0].PaymentMethodSystemName, store.Id);
            }

            //PaymentInfo

            //load payment method
            var paymentMethodSystemName = await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.SelectedPaymentMethodAttribute, store.Id);
            var paymentMethod = await _paymentPluginManager.LoadPluginBySystemNameAsync(paymentMethodSystemName, customer, store.Id);
            //if (paymentMethod == null)
            //    return RedirectToRoute("CheckoutPaymentMethod");

            //Check whether payment info should be skipped
            //if (paymentMethod.SkipPaymentInfo ||
            //    (paymentMethod.PaymentMethodType == PaymentMethodType.Redirection && _paymentSettings.SkipPaymentInfoStepForRedirectionPaymentMethods))
            //{
            //skip payment info page
            var paymentInfo = new ProcessPaymentRequest();

            //session save
            HttpContext.Session.Set("OrderPaymentInfo", paymentInfo);

            //    return RedirectToRoute("CheckoutConfirm");
            //}

            return View("~/Plugins/Misc.SimpleCheckOut/Views/SimpleCheckout.cshtml", model);
        }


        [HttpPost]
        [ValidateCaptcha]
        public virtual async Task<IActionResult> Login(SimpleCheckoutModel simpleCheckoutModel, bool captchaValid)
        {
            ////validate CAPTCHA
            //if (_captchaSettings.Enabled && _captchaSettings.ShowOnLoginPage && !captchaValid)
            //{
            //    ModelState.AddModelError("", await _localizationService.GetResourceAsync("Common.WrongCaptchaMessage"));
            //}

            var returnUrl = Url.RouteUrl("SimpleCheckout").ToString();
            var model = simpleCheckoutModel.LoginModel;

            if (ModelState.IsValid)
            {
                var customerUserName = model.Username?.Trim();
                var customerEmail = model.Email?.Trim();
                var userNameOrEmail = _customerSettings.UsernamesEnabled ? customerUserName : customerEmail;

                var loginResult = await _customerRegistrationService.ValidateCustomerAsync(userNameOrEmail, model.Password);
                switch (loginResult)
                {
                    case CustomerLoginResults.Successful:
                        {
                            var customer = _customerSettings.UsernamesEnabled
                                ? await _customerService.GetCustomerByUsernameAsync(customerUserName)
                                : await _customerService.GetCustomerByEmailAsync(customerEmail);

                            return await _customerRegistrationService.SignInCustomerAsync(customer, returnUrl, model.RememberMe);
                        }
                    case CustomerLoginResults.MultiFactorAuthenticationRequired:
                        {
                            var customerMultiFactorAuthenticationInfo = new CustomerMultiFactorAuthenticationInfo
                            {
                                UserName = userNameOrEmail,
                                RememberMe = model.RememberMe,
                                ReturnUrl = returnUrl
                            };
                            HttpContext.Session.Set(NopCustomerDefaults.CustomerMultiFactorAuthenticationInfo, customerMultiFactorAuthenticationInfo);
                            return RedirectToRoute("MultiFactorVerification");
                        }
                    case CustomerLoginResults.CustomerNotExist:
                        ModelState.AddModelError("", await _localizationService.GetResourceAsync("Account.Login.WrongCredentials.CustomerNotExist"));
                        break;
                    case CustomerLoginResults.Deleted:
                        ModelState.AddModelError("", await _localizationService.GetResourceAsync("Account.Login.WrongCredentials.Deleted"));
                        break;
                    case CustomerLoginResults.NotActive:
                        ModelState.AddModelError("", await _localizationService.GetResourceAsync("Account.Login.WrongCredentials.NotActive"));
                        break;
                    case CustomerLoginResults.NotRegistered:
                        ModelState.AddModelError("", await _localizationService.GetResourceAsync("Account.Login.WrongCredentials.NotRegistered"));
                        break;
                    case CustomerLoginResults.LockedOut:
                        ModelState.AddModelError("", await _localizationService.GetResourceAsync("Account.Login.WrongCredentials.LockedOut"));
                        break;
                    case CustomerLoginResults.WrongPassword:
                    default:
                        ModelState.AddModelError("", await _localizationService.GetResourceAsync("Account.Login.WrongCredentials"));
                        break;
                }
            }

            //If we got this far, something failed, redisplay form
            return RedirectToRoute("SimpleCheckout");
        }


        [HttpPost, ActionName("Index")]
        public virtual async Task<IActionResult> ConfirmOrder(SimpleCheckoutModel simpleCheckoutModel, IFormCollection form)
        {
            //validation
            if (_orderSettings.CheckoutDisabled)
                return RedirectToRoute("ShoppingCart");

            var customer = await _workContext.GetCurrentCustomerAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);

            if (!cart.Any())
                return RedirectToRoute("ShoppingCart");

            if (await _customerService.IsGuestAsync(customer) && !_orderSettings.AnonymousCheckoutAllowed)
                return Challenge();

            //custom address attributes
            var customAttributes = await _addressAttributeParser.ParseCustomAddressAttributesAsync(form);
            var customAttributeWarnings = await _addressAttributeParser.GetAttributeWarningsAsync(customAttributes);
            foreach (var error in customAttributeWarnings)
            {
                ModelState.AddModelError("", error);
            }

            //validate model
            //if (!ModelState.IsValid)
            //{
            //    //model is not valid. redisplay the form with errors
            //    var billingAddressModel = await _checkoutModelFactory.PrepareBillingAddressModelAsync(cart,
            //        selectedCountryId: newAddress.CountryId,
            //        overrideAttributesXml: customAttributes);
            //    billingAddressModel.NewAddressPreselected = true;
            //    return Json(new
            //    {
            //        update_section = new UpdateSectionJsonModel
            //        {
            //            name = "billing",
            //            html = await RenderPartialViewToStringAsync("OpcBillingAddress", billingAddressModel)
            //        },
            //        wrong_billing_address = true,
            //    });
            //}


            await UpdateBillingAddress(simpleCheckoutModel, customer, customAttributes);
            await UpdateShippingAddress(simpleCheckoutModel, customer, customAttributes);

            var confirmModel = await _checkoutModelFactory.PrepareConfirmOrderModelAsync(cart);
            try
            {
                //prevent 2 orders being placed within an X seconds time frame
                //if (!await IsMinimumOrderPlacementIntervalValidAsync(customer))
                //    throw new Exception(await _localizationService.GetResourceAsync("Checkout.MinOrderPlacementInterval"));

                //place order
                var processPaymentRequest = HttpContext.Session.Get<ProcessPaymentRequest>("OrderPaymentInfo");
                if (processPaymentRequest == null)
                {
                    processPaymentRequest = new ProcessPaymentRequest();
                    ////Check whether payment workflow is required
                    //if (await _orderProcessingService.IsPaymentWorkflowRequiredAsync(cart))
                    //    return RedirectToRoute("CheckoutPaymentInfo");
                }
                _paymentService.GenerateOrderGuid(processPaymentRequest);
                processPaymentRequest.StoreId = store.Id;
                processPaymentRequest.CustomerId = customer.Id;
                processPaymentRequest.PaymentMethodSystemName = await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.SelectedPaymentMethodAttribute, store.Id);
                HttpContext.Session.Set<ProcessPaymentRequest>("OrderPaymentInfo", processPaymentRequest);

                var placeOrderResult = await _orderProcessingService.PlaceOrderAsync(processPaymentRequest);
                if (placeOrderResult.Success)
                {
                    HttpContext.Session.Set<ProcessPaymentRequest>("OrderPaymentInfo", null);
                    var postProcessPaymentRequest = new PostProcessPaymentRequest
                    {
                        Order = placeOrderResult.PlacedOrder
                    };

                    try
                    {
                        await _paymentService.PostProcessPaymentAsync(postProcessPaymentRequest);
                        if (_webHelper.IsRequestBeingRedirected || _webHelper.IsPostBeingDone)
                        {
                            //redirection or POST has been done in PostProcessPayment
                            return Content(await _localizationService.GetResourceAsync("Checkout.RedirectMessage"));
                        }
                    }
                    catch (Exception)
                    {
                        await _orderProcessingService.ReOrderAsync(placeOrderResult.PlacedOrder);
                        await _orderProcessingService.DeleteOrderAsync(placeOrderResult.PlacedOrder);
                        throw;
                    }

                    return RedirectToRoute("CheckoutCompleted", new { orderId = placeOrderResult.PlacedOrder.Id });
                }

                foreach (var error in placeOrderResult.Errors)
                    confirmModel.Warnings.Add(error);
            }
            catch (Exception exc)
            {
                await _logger.WarningAsync(exc.Message, exc);
                confirmModel.Warnings.Add(exc.Message);
            }

            //If we got this far, something failed, redisplay form
            var model = await CreateSimpleCheckoutModel(customer, cart);
            model.ConfirmModel = confirmModel;
            return View("~/Plugins/Misc.SimpleCheckOut/Views/SimpleCheckout.cshtml", model);
        }

        private async Task UpdateBillingAddress(SimpleCheckoutModel simpleCheckoutModel, Customer customer, string customAttributes)
        {
            if (simpleCheckoutModel.SelectedBillingAddress.Id != 0)
            {
                //find address (ensure that it belongs to the current customer)
                var storedAddress = await _customerService.GetCustomerAddressAsync(customer.Id, simpleCheckoutModel.SelectedBillingAddress.Id);
                if (storedAddress == null)
                    throw new Exception("Address can't be loaded");

                storedAddress = Nop.Web.Extensions.MappingExtensions.ToEntity(simpleCheckoutModel.SelectedBillingAddress, storedAddress);
                //storedAddress = simpleCheckoutModel.SelectedBillingAddress.ToEntity(storedAddress);
                storedAddress.CustomAttributes = customAttributes;

                await _addressService.UpdateAddressAsync(storedAddress);

                customer.BillingAddressId = storedAddress.Id;
                await _customerService.UpdateCustomerAsync(customer);
            }
            else
            {
                //new address
                var newAddress = simpleCheckoutModel.SelectedBillingAddress;

                //try to find an address with the same values (don't duplicate records)
                var address = _addressService.FindAddress((await _customerService.GetAddressesByCustomerIdAsync(customer.Id)).ToList(),
                    newAddress.FirstName, newAddress.LastName, newAddress.PhoneNumber,
                    newAddress.Email, newAddress.FaxNumber, newAddress.Company,
                    newAddress.Address1, newAddress.Address2, newAddress.City,
                    newAddress.County, newAddress.StateProvinceId, newAddress.ZipPostalCode,
                    newAddress.CountryId, customAttributes);

                if (address == null)
                {
                    //address is not found. let's create a new one
                    address = Nop.Web.Extensions.MappingExtensions.ToEntity(newAddress);
                    address.CustomAttributes = customAttributes;
                    address.CreatedOnUtc = DateTime.UtcNow;

                    //some validation
                    if (address.CountryId == 0)
                        address.CountryId = null;

                    if (address.StateProvinceId == 0)
                        address.StateProvinceId = null;

                    await _addressService.InsertAddressAsync(address);
                    await _customerService.InsertCustomerAddressAsync(customer, address);
                }

                customer.BillingAddressId = address.Id;
                await _customerService.UpdateCustomerAsync(customer);
            }
        }

        private async Task UpdateShippingAddress(SimpleCheckoutModel simpleCheckoutModel, Customer customer, string customAttributes)
        {
            if (simpleCheckoutModel.BillingAddressModel.ShipToSameAddress)
            {
                customer.ShippingAddressId = customer.BillingAddressId;
            }
            else if (simpleCheckoutModel.SelectedShippingAddress.Id != 0)
            {
                //find address (ensure that it belongs to the current customer)
                var storedAddress = await _customerService.GetCustomerAddressAsync(customer.Id, simpleCheckoutModel.SelectedShippingAddress.Id);
                if (storedAddress == null)
                    throw new Exception("Address can't be loaded");

                storedAddress = Nop.Web.Extensions.MappingExtensions.ToEntity(simpleCheckoutModel.SelectedShippingAddress, storedAddress);
                storedAddress.CustomAttributes = customAttributes;

                await _addressService.UpdateAddressAsync(storedAddress);

                customer.ShippingAddressId = storedAddress.Id;
            }
            else
            {
                //new address
                var newAddress = simpleCheckoutModel.SelectedShippingAddress;

                //try to find an address with the same values (don't duplicate records)
                var address = _addressService.FindAddress((await _customerService.GetAddressesByCustomerIdAsync(customer.Id)).ToList(),
                    newAddress.FirstName, newAddress.LastName, newAddress.PhoneNumber,
                    newAddress.Email, newAddress.FaxNumber, newAddress.Company,
                    newAddress.Address1, newAddress.Address2, newAddress.City,
                    newAddress.County, newAddress.StateProvinceId, newAddress.ZipPostalCode,
                    newAddress.CountryId, customAttributes);

                if (address == null)
                {
                    //address is not found. let's create a new one
                    address = Nop.Web.Extensions.MappingExtensions.ToEntity(newAddress);
                    address.CustomAttributes = customAttributes;
                    address.CreatedOnUtc = DateTime.UtcNow;

                    //some validation
                    if (address.CountryId == 0)
                        address.CountryId = null;

                    if (address.StateProvinceId == 0)
                        address.StateProvinceId = null;

                    await _addressService.InsertAddressAsync(address);
                    await _customerService.InsertCustomerAddressAsync(customer, address);
                }
                customer.ShippingAddressId = address.Id;
            }
            await _customerService.UpdateCustomerAsync(customer);
        }

        private async Task<SimpleCheckoutModel> CreateSimpleCheckoutModel(Customer customer, System.Collections.Generic.IList<ShoppingCartItem> cart)
        {
            //model
            var anonymousCheckoutMandatory = _orderSettings.AnonymousCheckoutAllowed && _customerSettings.UserRegistrationType == UserRegistrationType.Disabled;
            var isGuest = await _customerService.IsGuestAsync(customer);
            var model = new SimpleCheckoutModel()
            {
                ShowLogin = !anonymousCheckoutMandatory && isGuest,
                ShowRegistration = isGuest && _customerSettings.UserRegistrationType != UserRegistrationType.Disabled,
                ShippingAddressModel = await _checkoutModelFactory.PrepareShippingAddressModelAsync(cart, prePopulateNewAddressWithCustomerFields: true),
                BillingAddressModel = await _checkoutModelFactory.PrepareBillingAddressModelAsync(cart, prePopulateNewAddressWithCustomerFields: true),
                ConfirmModel = await _checkoutModelFactory.PrepareConfirmOrderModelAsync(cart),
                LoginModel = await _customerModelFactory.PrepareLoginModelAsync(true),
                SelectedBillingAddress = null,
            };

            //Customer has existing BillingAddress specified, try to use that
            if (customer.BillingAddressId.HasValue)
            {
                //Check if address actually exists for customer, if so use that
                model.SelectedBillingAddress = model.BillingAddressModel.ExistingAddresses.FirstOrDefault(x => x.Id == customer.BillingAddressId);
            }
            if (model.SelectedBillingAddress == null)
            {
                model.SelectedBillingAddress = model.BillingAddressModel.ExistingAddresses.FirstOrDefault();
            }
            if(model.SelectedBillingAddress == null)
            {
                model.SelectedBillingAddress = model.BillingAddressModel.BillingNewAddress;
            }
            model.SelectedBillingAddress.AvailableCountries = model.BillingAddressModel.BillingNewAddress.AvailableCountries;
            model.SelectedBillingAddress.AvailableStates = model.BillingAddressModel.BillingNewAddress.AvailableStates;


            //Do not set Shipping Addres if same as billingaddress
            if(customer.ShippingAddressId!=customer.BillingAddressId)
            {
                if (customer.ShippingAddressId.HasValue)
                {
                    model.SelectedShippingAddress = model.ShippingAddressModel.ExistingAddresses.FirstOrDefault(x => x.Id == customer.ShippingAddressId);
                }
                if (model.SelectedShippingAddress == null)
                {
                    model.SelectedShippingAddress = model.ShippingAddressModel.ExistingAddresses.FirstOrDefault(x=>customer.BillingAddressId==null || x.Id!=customer.BillingAddressId);
                }
            }
            if (model.SelectedShippingAddress == null)
            {
                model.SelectedShippingAddress = model.ShippingAddressModel.ShippingNewAddress;
            }
            model.SelectedShippingAddress.AvailableCountries = model.ShippingAddressModel.ShippingNewAddress.AvailableCountries;
            model.SelectedShippingAddress.AvailableStates = model.ShippingAddressModel.ShippingNewAddress.AvailableStates;

            model.BillingAddressModel.ShipToSameAddress = model.SelectedShippingAddress == model.ShippingAddressModel.ShippingNewAddress;
            return model;
        }

        //Copied from nopCommerce CheckoutController.Index
        private async Task<IActionResult> ValidateCheckoutIsValid()
        {
            //validation
            if (_orderSettings.CheckoutDisabled)
                return RedirectToRoute("ShoppingCart");

            var customer = await _workContext.GetCurrentCustomerAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);

            if (!cart.Any())
                return RedirectToRoute("ShoppingCart");

            var cartProductIds = cart.Select(ci => ci.ProductId).ToArray();
            var downloadableProductsRequireRegistration =
                _customerSettings.RequireRegistrationForDownloadableProducts && await _productService.HasAnyDownloadableProductAsync(cartProductIds);

            if (await _customerService.IsGuestAsync(customer) && (!_orderSettings.AnonymousCheckoutAllowed || downloadableProductsRequireRegistration))
                return Challenge();

            //if we have only "button" payment methods available (displayed on the shopping cart page, not during checkout),
            //then we should allow standard checkout
            //all payment methods (do not filter by country here as it could be not specified yet)
            var paymentMethods = await (await _paymentPluginManager
                .LoadActivePluginsAsync(customer, store.Id))
                .WhereAwait(async pm => !await pm.HidePaymentMethodAsync(cart)).ToListAsync();
            //payment methods displayed during checkout (not with "Button" type)
            var nonButtonPaymentMethods = paymentMethods
                .Where(pm => pm.PaymentMethodType != PaymentMethodType.Button)
                .ToList();
            //"button" payment methods(*displayed on the shopping cart page)
            var buttonPaymentMethods = paymentMethods
                .Where(pm => pm.PaymentMethodType == PaymentMethodType.Button)
                .ToList();
            if (!nonButtonPaymentMethods.Any() && buttonPaymentMethods.Any())
                return RedirectToRoute("ShoppingCart");

            //reset checkout data
            await _customerService.ResetCheckoutDataAsync(customer, store.Id);

            //validation (cart)
            var checkoutAttributesXml = await _genericAttributeService.GetAttributeAsync<string>(customer,
                NopCustomerDefaults.CheckoutAttributes, store.Id);
            var scWarnings = await _shoppingCartService.GetShoppingCartWarningsAsync(cart, checkoutAttributesXml, true);
            if (scWarnings.Any())
                return RedirectToRoute("ShoppingCart");
            //validation (each shopping cart item)
            foreach (var sci in cart)
            {
                var product = await _productService.GetProductByIdAsync(sci.ProductId);

                var sciWarnings = await _shoppingCartService.GetShoppingCartItemWarningsAsync(customer,
                    sci.ShoppingCartType,
                    product,
                    sci.StoreId,
                    sci.AttributesXml,
                    sci.CustomerEnteredPrice,
                    sci.RentalStartDateUtc,
                    sci.RentalEndDateUtc,
                    sci.Quantity,
                    false,
                    sci.Id);
                if (sciWarnings.Any())
                    return RedirectToRoute("ShoppingCart");
            }

            return null;
        }

    }
}
