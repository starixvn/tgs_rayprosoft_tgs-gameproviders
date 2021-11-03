using System.Threading.Tasks;
using TGS.GameProviders.Core.Repositories.Models.Player;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Core.WMCasino
{
    public interface IWMCasinoService
    {
        ValueTask<string> GetGameProviderCultureCodeAsync(string cultureCode);

        ValueTask<GetPlayer> GetPlayerAsync(string playerCode);

        ValueTask<WMCasinoConfiguration> GetSettingConfigurationAsync();
    }
}
