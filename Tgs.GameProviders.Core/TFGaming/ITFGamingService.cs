using System.Threading.Tasks;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameProviders.Domains.SettingConfiguration.TF;

namespace Tgs.GameProviders.Core.TFGaming
{
    public interface ITFGamingService
    {
        ValueTask<TFBrandMerchant> GetBrandMerchantAsync(string brandCode, string currencyCode);

        ValueTask<TFGamingConfiguration> GetSettingConfigurationAsync();
    }
}
