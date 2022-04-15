namespace Tgs.GameProviders.Domains.SettingConfiguration.DreamGame
{
    public record DreamGameBetLimit()
    {
        public string Currency { get; init; } = default!;

        public string TableLimit { get; init; } = default!;
    }
}
