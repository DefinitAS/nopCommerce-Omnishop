using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Payments.PayPalStandard.Infrastructure
{
    public partial class RouteProvider : IRouteProvider
    {
        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="endpointRouteBuilder">Route builder</param>
        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapControllerRoute(
                "Plugin.Payments.CoinbaseCommerce.PaymentReturn", "Plugins/PaymentCoinbaseCommerce/PaymentReturn/{orderGuid}", 
                new { controller = "PaymentCoinbaseCommerce", action = "PaymentReturn" });
            endpointRouteBuilder.MapControllerRoute(
                "Plugin.Payments.CoinbaseCommerce.PaymentCancel", "Plugins/PaymentCoinbaseCommerce/PaymentCancel/{orderGuid}", 
                new { controller = "PaymentCoinbaseCommerce", action = "PaymentCancel" });
            endpointRouteBuilder.MapControllerRoute(
                "Plugin.Payments.CoinbaseCommerce.PaymentWebhook", "Plugins/PaymentCoinbaseCommerce/PaymentWebhook", 
                new { controller = "PaymentCoinbaseCommerce", action = "PaymentWebhook" });

        }

        /// <summary>
        /// Gets a priority of route provider
        /// </summary>
        public int Priority => -1;
    }
}