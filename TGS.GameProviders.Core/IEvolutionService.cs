using System;
using System.Threading.Tasks;
using TGS.GameProviders.Repositories.Domains.Models.SettingConfiguration;
using TGS.GameProviders.Repositories.Entities;

namespace TGS.GameProviders.Core
{
    public interface IEvolutionService
    {
        Task<GameProvider> GetGameProviderAsync();

        string GetGameProviderCode();

        ValueTask<string> GetGameProviderCultureCodeAsync(string cultureCode);

        ValueTask<string> GetGameProviderCurrencyCodeAsync(string currencyCode);

        Task<Guid> GetGameProviderIdAsync();

        Task<EvolutionConfiguration> GetSettingConfigurationAsync();
    }
}
