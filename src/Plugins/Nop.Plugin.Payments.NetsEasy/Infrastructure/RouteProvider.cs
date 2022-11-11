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
            endpointRouteBuilder.MapControllerRoute("Plugin.Payments.NetsEasy.PaymentReturn", "Plugins/PaymentNetsEasy/PaymentReturn/{orderId}", new { controller = "PaymentNetsEasy", action = "PaymentReturn" });
            endpointRouteBuilder.MapControllerRoute("Plugin.Payments.NetsEasy.PaymentCancel", "Plugins/PaymentNetsEasy/PaymentCancel/{orderId}", new { controller = "PaymentNetsEasy", action = "PaymentCancel" });

        }

        /// <summary>
        /// Gets a priority of route provider
        /// </summary>
        public int Priority => -1;
    }
}