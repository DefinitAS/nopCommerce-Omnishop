using Nop.Core.Configuration;

namespace Nop.Plugin.Payments.CoinbaseCommerce
{
    /// <summary>
    /// Represents settings of Coinbase Commerce payment plugin
    /// </summary>
    public class CoinbaseCommerceSettings : ISettings
    {
        public string ApiKey { get; set; }

        public string SharedSecretKey { get; set; } = "test-secret-key-blablabla";

    }
}
