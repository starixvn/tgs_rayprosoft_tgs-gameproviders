namespace Tgs.GameProviders.Domains.SettingConfiguration;

public record BTiSportsConfiguration
{
	public Uri ApiUrl { get; init; } = default!;
	public Uri DataApiUrl { get; init; } = default!;
	public Uri TokenApiUrl { get; init; } = default!;
	public Uri HistoryBetsApiUrl { get; init; } = default!;
	public Uri HistoryBetsPagingApiUrl { get; init; } = default!;
	public Uri OpenBetsApiUrl { get; init; } = default!;
	public Uri SportsbookURL { get; init; } = default!;
	public string AgentUserName { get; init; } = default!;
	public string AgentPassword { get; init; } = default!;
	public string DesktopLayout { get; init; } = default!;
	public string MobileLayout { get; init; } = default!;
}
