using System.Threading.Tasks;
using TGS.GameProviders.Domains.SettingConfiguration;
using TGS.GameProviders.Domains.SettingConfiguration.TF;

namespace TGS.GameProviders.Core.TFGaming
{
    public interface ITFGamingService
    {
        ValueTask<TFBrandMerchant> GetBrandMerchantAsync(string brandCode, string currencyCode);

        ValueTask<TFGamingConfiguration> GetSettingConfigurationAsync();
    }
}
