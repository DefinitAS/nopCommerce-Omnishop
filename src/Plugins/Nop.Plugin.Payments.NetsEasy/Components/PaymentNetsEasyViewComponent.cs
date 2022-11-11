using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Plugin.Payments.NetsEasy.Models;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Payments.NetsEasy.Components
{
    [ViewComponent(Name = "PaymentNetsEasy")]
    public class PaymentNetsEasyViewComponent : NopViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View("~/Plugins/Payments.NetsEasy/Views/PaymentInfo.cshtml");
        }
    }
}
