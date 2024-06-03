namespace Tgs.GameProviders.Domains.SettingConfiguration;

public record AsiaGamingConfiguration
{
	public string AgentCode { get; init; } = default!;

	public string ProductionId { get; init; } = default!;

	public string DesKey { get; init; } = default!;

	public string Md5Key { get; init; } = default!;

	public Uri LoginApiUrl { get; init; } = default!;

	public Uri ApiUrl { get; init; } = default!;

	public OppositeBetSetting[] OppositeBets { get; init; } = default!;

	public string[] LockGameTypes { get; init; } = default!;

	public string[] GameTypes { get; init; } = default!;

	public IDictionary<string, string> Descriptions { get; init; } = default!;
}
