using System.Threading.Tasks;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core
{
    public interface ISettingService
    {
        ValueTask<TConfiguration?> GetConfigurationAsync<TConfiguration>(string key) where TConfiguration : PluggableConfigurationBase;
    }
}
