namespace Tgs.GameProviders.Domains.SettingConfiguration;

public record JiliTransferConfiguration
{
	public Uri ApiUrl { get; init; } = default!;
	public string AgentId { get; init; } = default!;
	public string AgentKey { get; init; } = default!;
}
