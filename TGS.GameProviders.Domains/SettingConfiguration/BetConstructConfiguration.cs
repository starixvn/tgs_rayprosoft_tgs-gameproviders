namespace Tgs.GameProviders.Domains.SettingConfiguration;

public record BetConstructConfiguration : PluggableConfigurationBase
{
    public string SharedKey { get; init; } = default!;
    public string PartnerId { get; init; } = default!;
}
