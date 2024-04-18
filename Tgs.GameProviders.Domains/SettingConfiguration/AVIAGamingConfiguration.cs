namespace Tgs.GameProviders.Domains.SettingConfiguration;

public record AVIAGamingConfiguration
{
	public decimal AmountMaximum { get; set; } = default!;

	public string AmountFormat { get; set; } = default!;

	public Uri ApiUrl { get; init; } = default!;

	public string Authorization { get; set; } = default!;

	public string PlayerPassword { get; init; } = default!;
}
