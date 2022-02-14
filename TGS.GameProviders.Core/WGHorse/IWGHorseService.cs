using System;
using System.Threading.Tasks;
using Tgs.GameProviders.QueryServer.Models;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Core.WGHorse
{
    public interface IWGHorseService
    {
        RoundClientStatus GetClientRound(decimal playerWinLoss);

        ValueTask<GameInfo> GetGameByExternalGameId(Guid gameProviderId, string externalGameId);

        string GetGameProviderCode();

        ValueTask<string> GetGameProviderCultureCodeAsync(string cultureCode);

        ValueTask<string> GetGameProviderCurrencyCodeAsync(string currencyCode);

        ValueTask<Guid> GetGameProviderIdAsync();

        ValueTask<WGHorseConfiguration> GetSettingConfigurationAsync();
    }
}
