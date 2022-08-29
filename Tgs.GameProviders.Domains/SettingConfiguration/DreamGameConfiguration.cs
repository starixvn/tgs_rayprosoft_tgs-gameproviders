using Tgs.GameProviders.Domains.SettingConfiguration.DreamGame;

namespace Tgs.GameProviders.Domains.SettingConfiguration;

public record DreamGameConfiguration : PluggableConfigurationBase
{
    public Uri ApiUrl { get; init; } = default!;

    public Uri VideoGameApiUrl { get; init; } = default!;

    public string ApiKey { get; init; } = default!;
    public string AgentName { get; init; } = default!;

    public string PlayerPassword { get; init; } = default!;

    public DreamGameBetLimit[] BetLimits { get; init; } = default!;
}
