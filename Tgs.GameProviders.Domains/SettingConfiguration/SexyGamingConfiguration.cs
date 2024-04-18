using Tgs.GameProviders.Domains.SettingConfiguration.SexyGaming;

namespace Tgs.GameProviders.Domains.SettingConfiguration;

public record SexyGamingConfiguration : PluggableConfigurationBase
{
	public Uri ApiUrl { get; init; } = default!;
	public string AgentId { get; init; } = default!;
	public string Cert { get; init; } = default!;
	public SexyGamingBetLimit BetLimit { get; init; } = default!;
	public OppositeBetSetting[] OppositeBets { get; init; } = default!;
}
