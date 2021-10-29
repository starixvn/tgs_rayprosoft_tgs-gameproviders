using System.Threading.Tasks;
using TGS.GameProviders.Repositories.Domains.Models.SettingConfiguration;

namespace TGS.GameProviders.Core
{
    public interface ISBOService
    {
        ValueTask<SBOConfiguration> GetSettingConfigurationAsync();
    }
}