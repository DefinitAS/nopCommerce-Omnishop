using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Orders;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components
{
    [ViewComponent(Name = "SimpleCheckout_ShippingOptions")]
    public class ShippingOptionsComponent : NopViewComponent
    {
        private readonly ICheckoutModelFactory _checkoutModelFactory;
        private readonly ICustomerService _customerService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;

        public ShippingOptionsComponent(ICheckoutModelFactory checkoutModelFactory,
                        ICustomerService customerService,
                        IGenericAttributeService genericAttributeService,
                        IShoppingCartService shoppingCartService, 
                        IStoreContext storeContext,
                        IWorkContext workContext)
        {
            _checkoutModelFactory = checkoutModelFactory;
            _customerService = customerService;
            _genericAttributeService = genericAttributeService;
            _shoppingCartService = shoppingCartService;
            _storeContext = storeContext;
            _workContext = workContext;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var requestPath = HttpContext.Request.Path.ToString().ToLower();
            var customer = await _workContext.GetCurrentCustomerAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);

            if(requestPath.EndsWith("/cart"))
            {
                var model = await _checkoutModelFactory.PrepareShippingMethodModelAsync(cart, await _customerService.GetCustomerShippingAddressAsync(customer));

                var selectedShippingOption = await _genericAttributeService.GetAttributeAsync<ShippingOption>(customer, NopCustomerDefaults.SelectedShippingOptionAttribute, store.Id);
                var selectedPickupPoint = await _genericAttributeService.GetAttributeAsync<PickupPoint>(customer, NopCustomerDefaults.SelectedPickupPointAttribute, store.Id);
                if(selectedShippingOption==null)
                {
                    selectedShippingOption = model.ShippingMethods.FirstOrDefault(x=>x.Selected).ShippingOption;
                }

                if (selectedShippingOption.IsPickupInStore)
                {
                    var inputValue = ($"PickupPoint___{selectedPickupPoint.Id}___{selectedPickupPoint.ProviderSystemName}");
                    model.CustomProperties.Add("SelectedOption", inputValue);
                }
                else
                {
                    var inputValue = $"{selectedShippingOption.Name}___{selectedShippingOption.ShippingRateComputationMethodSystemName}";
                    model.CustomProperties.Add("SelectedOption", inputValue);
                }

                return View("~/Plugins/Misc.SimpleCheckOut/Views/SimpleCheckout_ShippingOptions.cshtml", model);
            }
            return Content("");


        }
    }
}
