using System.Threading.Tasks;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.SexyGaming;

public interface ISexyGamingService
{
    ValueTask<string> GetGameProviderCultureCodeAsync(string cultureCode);

    ValueTask<string> GetGameProviderCurrencyCodeAsync(string currencyCode);

    ValueTask<SexyGamingConfiguration> GetSettingConfigurationAsync();

    bool IsAccountIsNotExists(string status);

    bool IsSuccess(string status);
}
