using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Core.Domain.Localization;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Misc.Omnishop.Infrastructure
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

            endpointRouteBuilder.MapControllerRoute(name: "ImportAPIUpdateProduct",
                pattern: $"{lang}/importapiproducts/updateproduct/{{sku}}",
                defaults: new { controller = "ImportAPIProducts", action = "UpdateProduct" });

            endpointRouteBuilder.MapControllerRoute(name: "ImportAPIAddPictureToProduct",
                pattern: $"{lang}/importapiproducts/addpicturetoproduct/{{sku}}",
                defaults: new { controller = "ImportAPIProducts", action = "AddPictureToProduct" });

            endpointRouteBuilder.MapControllerRoute(name: "ImportAPIPing",
                pattern: $"{lang}" + "/importapiproducts/ping/{**requestSubPath}",
                defaults: new { controller = "ImportAPIProducts", action = "Ping" });
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