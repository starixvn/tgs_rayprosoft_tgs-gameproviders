namespace Tgs.GameProviders.Domains.SettingConfiguration;

public record AwcSgConfiguration
{
    public Uri ApiUrl { get; init; } = default!;
    public Uri ApiRecordUrl { get; init; } = default!;
}
