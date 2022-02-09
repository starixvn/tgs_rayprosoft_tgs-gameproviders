using System;
using System.Threading.Tasks;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Core.SexyGaming
{
    public interface ISexyGamingService
    {
        ValueTask<string> GetGameProviderCultureCodeAsync(string cultureCode);

        ValueTask<string> GetGameProviderCurrencyCodeAsync(string currencyCode);

        ValueTask<SexyGamingConfiguration> GetSettingConfigurationAsync();
    }
}
