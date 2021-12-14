using System;
using System.Threading.Tasks;
using TGS.GameProviders.Core.Repositories.Models.Game;
using TGS.GameProviders.Core.Repositories.Models.GameProvider;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Core.PragmaticPlay
{
    public interface IPragmaticPlayService
    {
        ValueTask<GameModel> GetGameAsync(int gameId);

        ValueTask<GameProviderModel> GetGameProviderAsync(Guid gameProviderId);

        ValueTask<string> GetGameProviderLanguageAsync(Guid gameProviderId, string cultureCode);

        ValueTask<PragmaticPlayConfiguration> GetSettingConfigurationAsync();
    }
}
