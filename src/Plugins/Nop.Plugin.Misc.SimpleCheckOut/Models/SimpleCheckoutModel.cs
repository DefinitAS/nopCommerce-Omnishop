using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Web.Models.Common;
using Nop.Web.Framework.Models;
using Nop.Web.Models.Checkout;
using Nop.Web.Models.Customer;

namespace Nop.Plugin.Misc.SimpleCheckOut.Models
{
    public partial record SimpleCheckoutModel : BaseNopModel
    {
        public SimpleCheckoutModel()
        {
        }

        public bool ShowLogin { get; set; }
        public bool ShowRegistration { get; set; }
        public AddressModel SelectedBillingAddress { get; set; }
        public AddressModel SelectedShippingAddress { get; set; }

        public LoginModel LoginModel { get; set; } 
        public CheckoutConfirmModel ConfirmModel { get; set; }
        public CheckoutBillingAddressModel BillingAddressModel { get; set; }
        public CheckoutShippingAddressModel ShippingAddressModel { get; set; }

    }
}
