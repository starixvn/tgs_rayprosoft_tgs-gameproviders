namespace Tgs.GameProviders.Domains.SettingConfiguration;

public record WicketCricketConfiguration
{
    public Uri ApiUrl { get; init; } = default!;
    public Uri ExchangeApiUrl { get; init; } = default!;
    public string Website { get; init; } = default!;
    public string Cert { get; init; } = default!;
    public string Agent { get; init; } = default!;
}
