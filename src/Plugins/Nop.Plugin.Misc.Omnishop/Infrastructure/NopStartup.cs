using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Plugin.Misc.Omnishop.Models;
using Nop.Plugin.Misc.Omnishop.Services;
using Nop.Services.Configuration;

namespace Nop.Plugin.Misc.Omnishop.Infrastructure
{
    /// <summary>
    /// Represents object for the configuring services on application startup
    /// </summary>
    public class NopStartup : INopStartup
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IOmnishopOrderService, OmnishopOrderService>();
        }

        public void Configure(IApplicationBuilder application)
        {   
            application.UseMiddleware<CanonicalRedirectMiddleware>();
        }

        /// <summary>
        /// Gets order of this startup configuration implementation
        /// </summary>
        public int Order => int.MinValue;
    }
}