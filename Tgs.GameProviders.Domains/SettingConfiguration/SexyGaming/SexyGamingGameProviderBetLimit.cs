namespace Tgs.GameProviders.Domains.SettingConfiguration.SexyGaming
{
    public record SexyGamingGameProviderBetLimit(Guid Id, string CurrencyCode, int MinBet, int MaxBet, int MaxBetPerMatch, string ExternalSetting = "");
}
