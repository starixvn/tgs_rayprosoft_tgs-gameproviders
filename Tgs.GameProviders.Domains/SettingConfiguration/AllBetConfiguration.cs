namespace Tgs.GameProviders.Domains.SettingConfiguration;

public record AllBetConfiguration
{
	public Uri ApiUrl { get; init; } = default!;

	public string BrandCode { get; init; } = default!;

	public string AgentId { get; init; } = default!;

	public string AllBetApiKey { get; init; } = default!;

	public string CurrencyCode { get; init; } = default!;

	public string PartnerApiKey { get; init; } = default!;

	public string PropertyId { get; init; } = default!;

	public string Suffix { get; init; } = default!;
}
