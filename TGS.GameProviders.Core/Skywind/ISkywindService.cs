using System.Threading.Tasks;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Core.Skywind
{
    public interface ISkywindService
    {
        ValueTask<string> GetGameProviderCultureCodeAsync(string cultureCode);

        ValueTask<SkywindConfiguration> GetSettingConfigurationAsync();
    }
}
