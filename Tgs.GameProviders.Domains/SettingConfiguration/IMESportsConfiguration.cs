namespace Tgs.GameProviders.Domains.SettingConfiguration;

public record IMESportsConfiguration
{
	public Uri ApiUrl { get; init; } = default!;
	public int ProductWallet { get; init; }
	public bool SupportHttps { get; init; } = false;
	public string BrandCode { get; init; } = default!;
	public string MerchantCode { get; init; } = default!;
	public string Prefix { get; init; } = default!;
}
