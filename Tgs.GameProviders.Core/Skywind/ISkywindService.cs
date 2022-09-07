using System.Threading.Tasks;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.Skywind;

public interface ISkywindService
{
    ValueTask<string> GetGameProviderCultureCodeAsync(string cultureCode);

    ValueTask<SkywindConfiguration> GetSettingConfigurationAsync();
}
