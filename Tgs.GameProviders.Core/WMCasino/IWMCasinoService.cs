using System.Threading.Tasks;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameProviders.Core.WMCasino
{
    public interface IWMCasinoService
    {
        ValueTask<string> GetGameProviderCultureCodeAsync(string cultureCode);

        ValueTask<PlayerModel> GetPlayerAsync(string playerCode);

        ValueTask<WMCasinoConfiguration> GetSettingConfigurationAsync();
    }
}
