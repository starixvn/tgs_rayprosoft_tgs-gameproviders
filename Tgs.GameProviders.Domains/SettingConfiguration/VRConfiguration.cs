using Tgs.GameProviders.Domains.SettingConfiguration.VR;

namespace Tgs.GameProviders.Domains.SettingConfiguration;

public record VRConfiguration : PluggableConfigurationBase
{
    /// <summary>
    /// The amount maximum
    /// </summary>
    public decimal AmountMaximum { get; init; }

    /// <summary>
    /// The amount format
    /// </summary>
    public string AmountFormat { get; init; } = default!;

    /// <summary>
    /// The brand merchants
    /// </summary>
    public VRBrandMerchant[] BrandMerchants { get; init; } = default!;
}
