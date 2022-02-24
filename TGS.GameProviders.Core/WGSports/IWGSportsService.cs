using System;
using System.Threading.Tasks;
using Tgs.GameProviders.QueryServer.Models;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Core.WGSports
{
    public interface IWGSportsService
    {
        decimal? GetAmount(string amountString);

        RoundClientStatus GetClientRound(string playerWinLoss);

        ValueTask<GameInfo> GetGameByExternalGameId(Guid gameProviderId, string externalGameId);

        string GetGameProviderCode();

        ValueTask<string> GetGameProviderCultureCodeAsync(string cultureCode);

        ValueTask<string> GetGameProviderCurrencyCodeAsync(string currencyCode);

        ValueTask<Guid> GetGameProviderIdAsync();

        ValueTask<WGSportsConfiguration> GetSettingConfigurationAsync();
    }
}
