using System;
using System.Threading.Tasks;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Core.Evolution
{
    public interface IEvolutionService
    {
        string GetGameProviderCode();

        ValueTask<string> GetGameProviderCultureCodeAsync(string cultureCode);

        ValueTask<string> GetGameProviderCurrencyCodeAsync(string currencyCode);

        Task<Guid> GetGameProviderIdAsync();

        ValueTask<EvolutionConfiguration> GetSettingConfigurationAsync();
    }
}
