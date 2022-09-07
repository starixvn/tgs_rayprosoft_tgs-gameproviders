using Tgs.GameProviders.Domains.SettingConfiguration.Jili;

namespace Tgs.GameProviders.Domains.SettingConfiguration;

public record JiliConfiguration : PluggableConfigurationBase
{
    public Uri ApiUrl { get; init; } = default!;

    public string ClientId { get; init; } = default!;

    public string SecretKey { get; init; } = default!;

    public IEnumerable<JiliMerchant> Merchants { get; init; } = default!;
}
