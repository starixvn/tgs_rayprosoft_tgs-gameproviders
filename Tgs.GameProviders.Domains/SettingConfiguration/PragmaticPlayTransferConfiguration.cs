namespace Tgs.GameProviders.Domains.SettingConfiguration;

public record PragmaticPlayTransferConfiguration
{
    public Uri ApiUrl { get; init; } = default!;
    public string SecretKey { get; init; } = default!;
    public string SecureLogin { get; init; } = default!;
}
