using System.Threading.Tasks;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Core
{
    public interface ISettingService
    {
        ValueTask<TConfiguration?> GetConfigurationAsync<TConfiguration>(string key) where TConfiguration : PluggableConfigurationBase;
    }
}
