namespace Tgs.GameProviders.Domains.SettingConfiguration;

public record JiliConfiguration
{
	public Uri ApiUrl { get; init; } = default!;

	public string ClientId { get; init; } = default!;

	public string SecretKey { get; init; } = default!;

	public string AgentId { get; init; } = default!;

	public string AgentKey { get; init; } = default!;

	public string BrandCode { get; init; } = default!;
}
