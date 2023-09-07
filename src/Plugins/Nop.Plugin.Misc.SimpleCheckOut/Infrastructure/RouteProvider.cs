using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Core.Domain.Localization;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Misc.SimpleCheckOut.Infrastructure
{
    public partial class RouteProvider : IRouteProvider
    {
        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="endpointRouteBuilder">Route builder</param>
        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            var lang = GetLanguageRoutePattern();

            endpointRouteBuilder.MapControllerRoute(name: "SimpleCheckout",
                pattern: $"{lang}/checkout",
                defaults: new { controller = "SimpleCheckout", action = "Index" });

            endpointRouteBuilder.MapControllerRoute(name: "CheckoutLogin",
                pattern: $"{lang}/checkout/login",
                defaults: new { controller = "SimpleCheckout", action = "Login" });

            //"Hijack" nopCommerce route to login page for checkout as guest
            //we set the route to simple checkout where login or continue as guest is part of checkout page
            endpointRouteBuilder.MapControllerRoute(name: "LoginCheckoutAsGuest",
                pattern: $"{lang}/checkout",
                defaults: new { controller = "SimpleCheckout", action = "Index", checkoutAsGuest = true });

            //Override all routes to checkout pages instead go to our simplecheckout page
            endpointRouteBuilder.MapControllerRoute(name: "Checkout",
                pattern: $"{lang}/checkout",
                defaults: new { controller = "SimpleCheckout", action = "Index" });

            endpointRouteBuilder.MapControllerRoute(name: "CheckoutOnePage",
                pattern: $"{lang}/checkout",
                defaults: new { controller = "SimpleCheckout", action = "Index" });

            endpointRouteBuilder.MapControllerRoute(name: "CheckoutShippingAddress",
                pattern: $"{lang}/checkout",
                defaults: new { controller = "SimpleCheckout", action = "Index" });

            endpointRouteBuilder.MapControllerRoute(name: "CheckoutBillingAddress",
                pattern: $"{lang}/checkout",
                defaults: new { controller = "SimpleCheckout", action = "Index" });

            endpointRouteBuilder.MapControllerRoute(name: "CheckoutShippingMethod",
                pattern: $"{lang}/checkout",
                defaults: new { controller = "SimpleCheckout", action = "Index" });

            endpointRouteBuilder.MapControllerRoute(name: "CheckoutPaymentMethod",
                pattern: $"{lang}/checkout",
                defaults: new { controller = "SimpleCheckout", action = "Index" });

            endpointRouteBuilder.MapControllerRoute(name: "CheckoutPaymentInfo",
                pattern: $"{lang}/checkout",
                defaults: new { controller = "SimpleCheckout", action = "Index" });

            endpointRouteBuilder.MapControllerRoute(name: "CheckoutConfirm",
                pattern: $"{lang}/checkout",
                defaults: new { controller = "SimpleCheckout", action = "Index" });

            //Shipping method widget on shoppingcartpage
            endpointRouteBuilder.MapControllerRoute(name: "CartShippingMethod",
                pattern: $"{lang}/cart/selectshippingmethod/{{shippingMethod}}",
                defaults: new { controller = "SimpleCheckout", action = "SelectShippingMethod" });

            //checkout/completed should still be routed to nopCommerce ordinary checkoutcontroller
            endpointRouteBuilder.MapControllerRoute(name: "CheckoutCompleted",
                pattern: $"{lang}/checkout/completed/{{orderId:int?}}",
                defaults: new { controller = "Checkout", action = "Completed" });

            endpointRouteBuilder.MapControllerRoute(name: "CheckoutCatchAll",
                pattern: $"{lang}" + "/checkout/{**requestSubPath}",
                defaults: new { controller = "SimpleCheckout", action = "Index" });
        }

        protected string GetLanguageRoutePattern()
        {
            if (DataSettingsManager.IsDatabaseInstalled())
            {
                var localizationSettings = EngineContext.Current.Resolve<LocalizationSettings>();
                if (localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
                {
                    //this pattern is set once at the application start, when we don't have the selected language yet
                    //so we use 'en' by default for the language value, later it'll be replaced with the working language code
                    var code = "en";
                    return $"{{{NopPathRouteDefaults.LanguageRouteValue}:maxlength(2):{NopPathRouteDefaults.LanguageParameterTransformer}={code}}}";
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets a priority of route provider
        /// </summary>
        public int Priority => 10;
    }
}