using Tgs.GameProviders.Domains.SettingConfiguration.Card365;

namespace Tgs.GameProviders.Domains.SettingConfiguration;

public record Card365Configuration : PluggableConfigurationBase
{
    public Uri ApiUrl { get; init; } = default!;

    public string Cert { get; init; } = default!;

    public string Website { get; init; } = default!;

    public IEnumerable<Card365BrandAgent> BrandAgents { get; init; } = default!;
}
