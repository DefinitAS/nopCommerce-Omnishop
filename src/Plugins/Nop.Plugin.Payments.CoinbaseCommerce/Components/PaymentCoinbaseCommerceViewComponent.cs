using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Plugin.Payments.CoinbaseCommerce.Models;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Payments.CoinbaseCommerce.Components
{
    [ViewComponent(Name = "PaymentCoinbaseCommerce")]
    public class PaymentCoinbaseCommerceViewComponent : NopViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View("~/Plugins/Payments.CoinbaseCommerce/Views/PaymentInfo.cshtml");
        }
    }
}
