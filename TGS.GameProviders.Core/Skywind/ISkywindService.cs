using System;
using System.Threading.Tasks;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Core.Skywind
{
    public interface ISkywindService
    {
        string GetGameProviderCode();

        ValueTask<string> GetGameProviderCultureCodeAsync(string cultureCode);

        ValueTask<Guid> GetGameProviderIdAsync();

        Task<SkywindConfiguration> GetSettingConfigurationAsync();
    }
}
