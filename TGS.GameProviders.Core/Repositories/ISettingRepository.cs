using System.Threading.Tasks;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Core.Repositories
{
    public interface ISettingRepository
    {
        Task<TConfiguration?> GetConfigurationAsync<TConfiguration>(string key) where TConfiguration : PluggableConfigurationBase;
    }
}
