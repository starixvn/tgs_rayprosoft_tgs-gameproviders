using Tgs.GameProviders.Domains.SettingConfiguration.Joker;

namespace Tgs.GameProviders.Domains.SettingConfiguration;

public record JokerConfiguration : PluggableConfigurationBase
{
    public Uri ApiUrl { get; init; } = default!;
    public Uri GameLaunchUrl { get; init; } = default!;
    public string AppId { get; init; } = default!;
    public string SecretKey { get; init; } = default!;

    public IEnumerable<JokerBrandMerchant> BrandMerchants { get; init; } = Array.Empty<JokerBrandMerchant>();
}
