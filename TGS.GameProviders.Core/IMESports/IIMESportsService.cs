using System;
using System.Threading.Tasks;
using TGS.GameProviders.Core.Repositories.Models.GameProvider;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Core.IMESports
{
    public interface IIMESportsService
    {
        ValueTask<GameInfo> GetGameAsync(int gameId);

        ValueTask<string> GetGameProviderCurrencyCodeAsync(Guid gameProviderId, string currencyCode);

        ValueTask<Guid> GetGameProviderIdAsync();

        ValueTask<string> GetGameProviderLanguageCodeAsync(Guid gameProviderId, string cultureCode);

        ValueTask<PlayerInfo> GetPlayerAsync(string playerCode);

        ValueTask<IMESportsConfiguration> GetSettingConfigurationAsync();
    }
}
