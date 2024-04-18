namespace Tgs.GameProviders.Domains.SettingConfiguration;

public record PinnacleConfiguration
{
	public Uri ApiUrl { get; init; } = default!;
	public string AgentCode { get; init; } = default!;
	public string AgentKey { get; init; } = default!;
	public string SecretKey { get; init; } = default!;
}
