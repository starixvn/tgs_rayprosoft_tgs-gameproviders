namespace Tgs.GameProviders.Domains.SettingConfiguration;

public class InplayMatrixESportsConfiguration
{
    public Uri ApiUrl { get; init; } = default!;

    public string MerchantCode { get; init; } = default!;

    public string MerchantPrefix { get; init; } = default!;

    public string Password { get; init; } = default!;

    public Uri ReportUrl { get; init; } = default!;
}
