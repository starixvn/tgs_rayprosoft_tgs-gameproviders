using System.Threading.Tasks;
using TGS.GameProviders.Domains.SettingConfiguration;
using TGS.GameProviders.Domains.SettingConfiguration.TF;

namespace TGS.GameProviders.Core.TFGaming
{
    public interface ITFGamingService
    {
        Task<TFBrandMerchant> GetBrandMerchantAsync(string brandCode, string currencyCode);

        Task<TFGamingConfiguration> GetSettingConfigurationAsync();
    }
}