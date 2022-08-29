using Tgs.GameProviders.Domains.SettingConfiguration.TF;

namespace Tgs.GameProviders.Domains.SettingConfiguration;

public record TFGamingConfiguration : PluggableConfigurationBase
{
    /// <summary>
    /// The brand merchants
    /// </summary>
    public TFBrandMerchant[] BrandMerchants { get; init; } = default!;
}
