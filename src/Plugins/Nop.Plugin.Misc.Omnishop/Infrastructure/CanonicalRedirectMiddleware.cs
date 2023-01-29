using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nop.Core;
using Nop.Plugin.Misc.Omnishop.Models;
using Nop.Services.Configuration;

namespace Nop.Plugin.Misc.Omnishop.Infrastructure
{
    internal class CanonicalRedirectMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly ILogger _logger;
        OmnishopPluginSettings _pluginSettings;

        /// <summary>
        /// Initializes <see cref="HttpsRedirectionMiddleware" />.
        /// </summary>
        /// <param name="next"></param>
        /// <param name="options"></param>
        /// <param name="config"></param>
        /// <param name="loggerFactory"></param>
        public CanonicalRedirectMiddleware(RequestDelegate next,  ILoggerFactory loggerFactory, ISettingService settingService, IStoreContext storeContext)
        {
            ArgumentNullException.ThrowIfNull(next);

            _next = next;

            _logger = loggerFactory.CreateLogger<CanonicalRedirectMiddleware>();
            _settingService = settingService;
            _storeContext = storeContext;
        }

        public async Task Invoke(HttpContext context)
        {
            var request = context.Request;
            if(request.Host.Host=="localhost")
            {
                await _next(context);
                return;
            }

            if (_pluginSettings==null)
            {
                var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
                _pluginSettings = await _settingService.LoadSettingAsync<OmnishopPluginSettings>(storeId);
            }

            if(_pluginSettings.ForceCanonicalUrl)
            {
                var store = _storeContext.GetCurrentStore();
                var storeUri = new Uri(store.Url);
                if (request.Host.ToString() != storeUri.Host)
                {
                    var redirectUrl = UriHelper.BuildAbsolute(
                        "https",
                        new HostString(storeUri.Host),
                        request.PathBase,
                        request.Path,
                        request.QueryString);

                    context.Response.StatusCode = StatusCodes.Status301MovedPermanently;
                    context.Response.Headers.Location = redirectUrl;
                    return;

                    //_logger.RedirectingToHttps(redirectUrl);
                }
            }


            await _next(context);

        }
    }
}
