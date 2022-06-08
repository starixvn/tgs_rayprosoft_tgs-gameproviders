namespace Tgs.GameProviders.Domains.SettingConfiguration
{
    public record AwcSpadeConfiguration
    {
        public Uri ApiUrl { get; init; } = default!;
        public Uri ApiRecordUrl { get; init; } = default!;
    }
}
