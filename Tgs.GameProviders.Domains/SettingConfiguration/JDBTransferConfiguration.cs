namespace Tgs.GameProviders.Domains.SettingConfiguration;

public record JDBTransferConfiguration
{
    public Uri ApiUrl { get; init; } = default!;

    public string Domain { get; init; } = default!;

    public string Agent { get; init; } = default!;

    public string AesIV { get; init; } = default!;

    public string AesKey { get; init; } = default!;

}