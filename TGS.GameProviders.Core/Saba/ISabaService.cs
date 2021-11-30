using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Core.Saba
{
    public interface ISabaService
    {
        ValueTask<string> GetGameProviderCurrencyCodeAsync(Guid gameProviderId, string currencyCode);

        ValueTask<Guid> GetGameProviderIdAsync();

        ValueTask<string> GetGameProviderLanguageCodeAsync(Guid gameProviderId, string cultureCode);

        ValueTask<SabaConfiguration> GetSettingConfigurationAsync();

        bool IsTestEnvironment(IEnumerable<string> testEnvironments, string currentEnvironmentName);
    }
}