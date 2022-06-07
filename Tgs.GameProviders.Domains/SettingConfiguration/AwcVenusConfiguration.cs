namespace Tgs.GameProviders.Domains.SettingConfiguration
{
    public record AwcVenusConfiguration
    {
        public Uri ApiUrl { get; init; } = default!;
        public Uri ApiRecordUrl { get; init; } = default!;
    }
}
