namespace Tgs.GameProviders.Domains.SettingConfiguration;

public record HabaneroTransferConfiguration
{
    public Uri ApiUrl { get; init; } = default!;
    public Uri GameApiUrl { get; init; } = default!;
    public string BrandId { get; init; } = default!;
    public string ApiKey { get; init; } = default!;
}
