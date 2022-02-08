using System.Linq;
using System.Threading.Tasks;
using TGS.GameProviders.Core;
using TGS.GameProviders.Core.TFGaming;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.SettingConfiguration;
using TGS.GameProviders.Domains.SettingConfiguration.TF;

namespace TGS.GameProviders.Services.TFGaming
{
    internal class TFGamingService : ITFGamingService
    {
        private readonly ISettingService _settingService;

        public TFGamingService(ISettingService settingService)
        {
            _settingService = settingService ?? throw new System.ArgumentNullException(nameof(settingService));
        }

        public async ValueTask<TFBrandMerchant> GetBrandMerchantAsync(string brandCode, string currencyCode)
        {
            var configuration = await _settingService.GetConfigurationAsync<TFGamingConfiguration>(GameProviderConfigurations.TFGaming).ConfigureAwait(false);
            var brandMerchant = configuration.BrandMerchants.Where(brand => brand.BrandCode == brandCode && brand.BrandCurrency == currencyCode).FirstOrDefault();

            if (brandMerchant == null)
            {
                brandMerchant = configuration.BrandMerchants.Where(brand => brand.BrandCode == "Default" && brand.BrandCurrency == currencyCode).FirstOrDefault();
            }
            return brandMerchant;
        }

        public ValueTask<TFGamingConfiguration> GetSettingConfigurationAsync()
            => _settingService.GetConfigurationAsync<TFGamingConfiguration>(GameProviderConfigurations.TFGaming);
    }
}
