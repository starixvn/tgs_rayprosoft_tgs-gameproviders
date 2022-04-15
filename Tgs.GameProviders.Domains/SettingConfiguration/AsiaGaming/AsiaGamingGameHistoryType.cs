namespace Tgs.GameProviders.Domains.SettingConfiguration.AsiaGaming
{
    public record AsiaGamingGameHistoryType
    {
        public string[] GameTypes { get; init; } = default!;

        public IDictionary<string, string> Descriptions { get; init; } = default!;
    }
}
