using System.Threading.Tasks;
using TGS.GameProviders.Repositories.Domains.Models.Player;
using TGS.GameProviders.Repositories.Domains.Models.SettingConfiguration;

namespace TGS.GameProviders.Core
{
    public interface IWMCasinoService
    {
        ValueTask<WMCasinoConfiguration> GetSettingConfigurationAsync();

        ValueTask<GetPlayer> GetPlayerAsync(string playerCode);

        ValueTask<string> GetGameProviderCultureCodeAsync(string cultureCode);
    }
}