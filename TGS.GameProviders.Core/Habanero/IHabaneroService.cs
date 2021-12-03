using System;
using System.Threading.Tasks;
using TGS.GameProviders.Core.Repositories.Models.Game;
using TGS.GameProviders.Core.Repositories.Models.GameProvider;
using TGS.GameProviders.Core.Repositories.Models.Player;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Core.Habanero
{
    public interface IHabaneroService
    {
        ValueTask<GameModel> GetGameAsync(int gameId);

        ValueTask<string> GetGameDomainAsync(Guid brandId, Guid gameProviderId, PlatformType platformType);

        ValueTask<GameProviderModel> GetGameProviderAsync(Guid gameProviderId);

        ValueTask<string> GetGameProviderLanguageCodeAsync(Guid gameProviderId, string cultureCode);

        ValueTask<PlayerModel> GetPlayerAsync(string playerCode);

        ValueTask<HabaneroConfiguration> GetSettingConfigurationAsync();
    }
}
