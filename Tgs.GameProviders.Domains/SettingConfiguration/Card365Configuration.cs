namespace Tgs.GameProviders.Domains.SettingConfiguration;

public record Card365Configuration
{
	public Uri ApiUrl { get; init; } = default!;

	public string Cert { get; init; } = default!;

	public string Website { get; init; } = default!;

	public string BrandCode { get; init; } = default!;
	public string AgentId { get; init; } = default!;
}
