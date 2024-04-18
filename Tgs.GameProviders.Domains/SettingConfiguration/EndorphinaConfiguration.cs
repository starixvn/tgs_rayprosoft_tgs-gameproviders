namespace Tgs.GameProviders.Domains.SettingConfiguration;
public record EndorphinaConfiguration
{
	public Uri ApiUrl { get; init; } = default!;
	public string MerchantId { get; init; } = default!;
	public string SecretKey { get; init; } = default!;
}
