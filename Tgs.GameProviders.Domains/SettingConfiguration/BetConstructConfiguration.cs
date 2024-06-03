namespace Tgs.GameProviders.Domains.SettingConfiguration;

public record BetConstructConfiguration
{
	public Uri ApiUrl { get; init; } = default!;
	public string SharedKey { get; init; } = default!;
	public string PartnerId { get; init; } = default!;
}
