using Tgs.GameProviders.Domains.SettingConfiguration.IMESports;

namespace Tgs.GameProviders.Domains.SettingConfiguration;

public record IMESportsConfiguration : PluggableConfigurationBase
{
    public IMESportsBrandMerchant[] BrandMerchants { get; init; } = default!;
    public int ProductWallet { get; init; }
    public bool SupportHttps { get; init; } = false;
}
