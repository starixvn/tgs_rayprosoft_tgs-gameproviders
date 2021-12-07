using System.Threading.Tasks;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Core.BetConstruct
{
    public interface IBetConstructService
    {
        ValueTask<BetConstructConfiguration> GetSettingConfigurationAsync();
    }
}