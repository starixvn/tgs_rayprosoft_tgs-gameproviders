using System.Threading.Tasks;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.BetConstruct;

public interface IBetConstructService
{
    ValueTask<BetConstructConfiguration> GetSettingConfigurationAsync();
}
