namespace Tgs.GameProviders.Domains.SettingConfiguration;

public record class CRSportsConfiguration
{
	public Uri ApiUrl { get; init; } = default!;
	public int AgentId { get; init; } = default!;
	public string SecretKey { get; init; } = default!;
	public string AgentAccount { get; init; } = default!;
	public string AgentPassword { get; init; } = default!;
}
