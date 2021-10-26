using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGS.GameProviders.Repositories.Domains.Models.SettingConfiguration;
using TGS.GameProviders.Repositories.Entities;

namespace TGS.GameProviders.Core
{
    public interface IEvolutionService
    {
        Task<GameProvider> GetGameProviderAsync();

        string GetGameProviderCode();

        Task<Guid> GetGameProviderIdAsync();

        Task<EvolutionConfiguration> GetSettingConfigurationAsync();

        ValueTask<string> GetGameProviderCurrencyCodeAsync(string currencyCode);

        ValueTask<string> GetGameProviderCultureCodeAsync(string cultureCode);
    }
}
