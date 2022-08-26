namespace Tgs.GameProviders.Domains.SettingConfiguration.IMESports;

public record IMESportsBrandMerchant
{
    public Uri ApiUrl { get; init; } = default!;
    public string BrandCode { get; init; } = default!;
    public string MerchantCode { get; init; } = default!;
    public string Prefix { get; init; } = default!;
}
