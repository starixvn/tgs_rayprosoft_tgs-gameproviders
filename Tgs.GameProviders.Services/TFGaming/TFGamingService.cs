using System.Linq;
using System.Threading.Tasks;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Core.TFGaming;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameProviders.Domains.SettingConfiguration.TF;

namespace Tgs.GameProviders.Services.TFGaming;

internal class TFGamingService : ITFGamingService
{
    private readonly ISettingRepository _settingService;

    public TFGamingService(ISettingRepository settingService)
    {
        _settingService = settingService ?? throw new System.ArgumentNullException(nameof(settingService));
    }

    public async ValueTask<TFBrandMerchant> GetBrandMerchantAsync(string brandCode, string currencyCode)
    {
        var configuration = await _settingService.GetConfigurationAsync<TFGamingConfiguration>(GameProviderConfigurations.TFGaming).ConfigureAwait(false);

        return configuration.BrandMerchants.Where(brand => brand.BrandCode == brandCode && brand.BrandCurrency == currencyCode).FirstOrDefault() ??
            configuration.BrandMerchants.Where(brand => brand.BrandCode == DefConst.BrandCodeDefault && brand.BrandCurrency == currencyCode).FirstOrDefault();
    }
}
