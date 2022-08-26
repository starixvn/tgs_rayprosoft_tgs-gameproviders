using System.Threading.Tasks;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameProviders.Domains.SettingConfiguration.TCG;

namespace Tgs.GameProviders.Core.TCG;

public interface ITCGService
{
    ValueTask<string> GetGameProviderCultureCodeAsync(string cultureCode);

    ValueTask<TCGMerchant> GetMerchantAsync(string currencyCode);

    ValueTask<TCGConfiguration> GetSettingConfigurationAsync();
}
