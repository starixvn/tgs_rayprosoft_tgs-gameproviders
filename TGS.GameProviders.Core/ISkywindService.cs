using System;
using System.Threading.Tasks;
using TGS.GameProviders.Repositories.Domains.Models.SettingConfiguration;
using TGS.GameProviders.Repositories.Entities;

namespace TGS.GameProviders.Core
{
    public interface ISkywindService
    {
        Task<GameProvider> GetGameProviderAsync();

        string GetGameProviderCode();

        ValueTask<string> GetGameProviderCultureCodeAsync(string cultureCode);

        Task<Guid> GetGameProviderIdAsync();

        Task<SkywindConfiguration> GetSettingConfigurationAsync();
    }
}
