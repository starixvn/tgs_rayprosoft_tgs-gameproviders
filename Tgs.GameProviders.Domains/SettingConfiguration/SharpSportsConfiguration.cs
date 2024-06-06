namespace Tgs.GameProviders.Domains.SettingConfiguration;
public record SharpSportsConfiguration
{
	public Uri ApiUrl { get; init; } = default!;
	public string MerchantId { get; init; } = default!;
	public string ClientId { get; init; } = default!;
	public string ClientSecret { get; init; } = default!;
}
