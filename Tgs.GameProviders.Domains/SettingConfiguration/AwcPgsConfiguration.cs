namespace Tgs.GameProviders.Domains.SettingConfiguration;

public record AwcPgsConfiguration
{
    public Uri ApiUrl { get; init; } = default!;
    public Uri ApiRecordUrl { get; init; } = default!;
}
