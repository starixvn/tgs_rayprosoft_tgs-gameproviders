namespace Tgs.GameProviders.Domains.SettingConfiguration;

public record WMCasinoTransferConfiguration
{
    public Uri ApiUrl { get; init; } = default!;
}
