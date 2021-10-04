using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGS.GameProviders.Domains.SettingConfiguration;
using TGS.GameProviders.Repositories.Entities;

namespace TGS.GameProviders.Core
{
    public interface ISexyGamingService
    {
       
        Task<GameProvider> GetGameProviderAsync();

        string GetGameProviderCode();

        Task<Guid> GetGameProviderIdAsync();

        Task<SexyGamingConfiguration> GetSettingConfigurationAsync();
    }
}
