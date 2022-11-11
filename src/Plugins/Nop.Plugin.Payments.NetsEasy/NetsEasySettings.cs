using Nop.Core.Configuration;

namespace Nop.Plugin.Payments.NetsEasy
{
    /// <summary>
    /// Represents settings of Nets Easy payment plugin
    /// </summary>
    public class NetsEasySettings : ISettings
    {
        public string MerchantId { get; set; }

        public string SecretKey { get; set; } = "test-secret-key-blablabla";

        public string CheckoutKey { get; set; }

        public string Url { get; set; } = "https://test.api.dibspayment.eu/";

    }
}
