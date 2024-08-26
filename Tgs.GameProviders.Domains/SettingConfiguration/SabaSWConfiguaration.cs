namespace Tgs.GameProviders.Domains.SettingConfiguration;

public record SabaSWConfiguration
{
	public Uri ApiUrl { get; init; } = default!;

	public string OperatorId { get; init; } = default!;

	public string VendorId { get; init; } = default!;

	public int OddsType { get; init; } = default!;
}
