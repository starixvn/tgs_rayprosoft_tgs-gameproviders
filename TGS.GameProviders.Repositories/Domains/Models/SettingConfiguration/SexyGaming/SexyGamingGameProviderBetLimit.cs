using System;

namespace TGS.GameProviders.Repositories.Domains.Models.SettingConfiguration.SexyGaming
{
    public record SexyGamingGameProviderBetLimit(Guid Id, string CurrencyCode, int MinBet, int MaxBet, int MaxBetPerMatch, string ExternalSetting = "");
}
