using System;
using System.Threading.Tasks;
using TGS.GameProviders.Core.Repositories.Models.GameProvider;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Core.WMCasino
{
    public interface IWMCasinoService
    {
        ValueTask<GameInfo> GetGameAsync(int gameId);

        ValueTask<GameProviderModel> GetGameProviderAsync(Guid gameProviderId);

        ValueTask<string> GetGameProviderCultureCodeAsync(string cultureCode);

        ValueTask<PlayerInfo> GetPlayerAsync(string playerCode);

        ValueTask<WMCasinoConfiguration> GetSettingConfigurationAsync();
    }
}
