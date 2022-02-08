using System;
using System.Threading.Tasks;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Core.WGHorse
{
    public interface IWGHorseService
    {
        ClientRoundStatus GetClientRound(decimal playerWinLoss);

        ValueTask<GameInfo> GetGameByExternalGameId(Guid gameProviderId, string externalGameId);

        string GetGameProviderCode();

        ValueTask<string> GetGameProviderCultureCodeAsync(string cultureCode);

        ValueTask<string> GetGameProviderCurrencyCodeAsync(string currencyCode);

        ValueTask<Guid> GetGameProviderIdAsync();

        Task<WGHorseConfiguration> GetSettingConfigurationAsync();
    }
}
