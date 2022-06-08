namespace Tgs.GameProviders.Domains.SettingConfiguration
{
    public record AwcKmConfiguration
    {
        public Uri ApiUrl { get; init; } = default!;
        public Uri ApiRecordUrl { get; init; } = default!;
    }
}
