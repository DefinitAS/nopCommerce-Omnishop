using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Payments.CoinbaseCommerce.Models
{
    public record PaymentInfoModel : BaseNopModel
    {
        public string Description { get; set; }
    }
}