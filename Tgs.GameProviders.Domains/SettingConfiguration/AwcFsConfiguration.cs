namespace Tgs.GameProviders.Domains.SettingConfiguration;

public record AwcFsConfiguration
{
    public Uri ApiUrl { get; init; } = default!;
    public Uri ApiRecordUrl { get; init; } = default!;
}
