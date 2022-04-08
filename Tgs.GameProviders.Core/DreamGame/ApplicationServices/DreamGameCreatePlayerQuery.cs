using System;
using Tgs.GameProviders.Domains.SettingConfiguration.DreamGame;

namespace Tgs.GameProviders.Core.DreamGame.ApplicationServices
{
    public record DreamGameCreatePlayerQuery(
        Uri ApiUrl,
        string ApiKey,
        string AgentName,
        string PlayerCode,
        string Password,
        string CurrencyCode,
        string GameProviderCurrencyCode,
        DreamGameBetLimit[] BetLimits);
}
