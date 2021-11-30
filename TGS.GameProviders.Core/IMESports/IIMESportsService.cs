using System;
using System.Threading.Tasks;
using TGS.GameProviders.Core.Repositories.Models.Game;
using TGS.GameProviders.Core.Repositories.Models.Player;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Core.IMESports
{
    public interface IIMESportsService
    {
        ValueTask<Guid> GetBrandIdAsync(string brandCode);

        ValueTask<GameModel> GetGameAsync(int gameId);

        ValueTask<string> GetGameProviderCurrencyCodeAsync(Guid gameProviderId, string currencyCode);

        ValueTask<Guid> GetGameProviderIdAsync();

        ValueTask<string> GetGameProviderLanguageCodeAsync(Guid gameProviderId, string cultureCode);

        ValueTask<PlayerModel> GetPlayerAsync(string playerCode);

        ValueTask<IMESportsConfiguration> GetSettingConfigurationAsync();
    }
}