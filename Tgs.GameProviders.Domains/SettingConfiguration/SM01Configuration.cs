namespace Tgs.GameProviders.Domains.SettingConfiguration;

public record SM01Configuration
{
	public Uri ApiUrl { get; init; } = default!;
	public string MerchantId { get; init; } = default!;
	public string MerchantKey { get; init; } = default!;
}
