using TGS.GameProviders.Domains.SettingConfiguration.IMESports;

namespace TGS.GameProviders.Domains.SettingConfiguration
{
    public record IMESportsConfiguration : PluggableConfigurationBase
    {
        public IMESportsBrandMerchant[] BrandMerchants { get; init; } = default!;
        public int ProductWallet { get; init; }
        public bool SupportHttps { get; init; } = false;
    }
}
