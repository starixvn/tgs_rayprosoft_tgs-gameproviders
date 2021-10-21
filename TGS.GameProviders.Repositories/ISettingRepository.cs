using System.Threading.Tasks;
using TGS.GameProviders.Repositories.Domains.Models.SettingConfiguration;
using TGS.GameProviders.Repositories.Entities;

namespace TGS.GameProviders.Repositories
{
    public interface ISettingRepository
    {
        Task<TConfiguration?> GetConfigurationAsync<TConfiguration>(string key) where TConfiguration : PluggableConfigurationBase;

        Task<Setting> GetSettingAsync<TConfiguration>(string key);
    }
}
