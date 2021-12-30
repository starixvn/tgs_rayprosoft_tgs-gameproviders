using System.Linq;
using System.Threading.Tasks;
using TGS.GameProviders.Core.Repositories;
using TGS.GameProviders.Core.TFGaming;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.SettingConfiguration;
using TGS.GameProviders.Domains.SettingConfiguration.TF;

namespace TGS.GameProviders.Services.TFGaming
{
    internal class TFGamingService : ITFGamingService
    {
        private readonly ISettingRepository _settingRepository;

        public TFGamingService(ISettingRepository settingRepository)
        {
            _settingRepository = settingRepository ?? throw new System.ArgumentNullException(nameof(settingRepository));
        }

        public async Task<TFBrandMerchant> GetBrandMerchantAsync(string brandCode, string currencyCode)
        {
            var configuration = await _settingRepository.GetConfigurationAsync<TFGamingConfiguration>(GameProviderConfigurations.TFGaming).ConfigureAwait(false);
            var brandMerchant = configuration.BrandMerchants.Where(brand => brand.BrandCode == brandCode && brand.BrandCurrency == currencyCode).FirstOrDefault();

            if (brandMerchant == null)
            {
                brandMerchant = configuration.BrandMerchants.Where(brand => brand.BrandCode == "Default" && brand.BrandCurrency == currencyCode).FirstOrDefault();
            }
            return brandMerchant;
        }

        public Task<TFGamingConfiguration> GetSettingConfigurationAsync()
            => _settingRepository.GetConfigurationAsync<TFGamingConfiguration>(GameProviderConfigurations.TFGaming);
    }
}