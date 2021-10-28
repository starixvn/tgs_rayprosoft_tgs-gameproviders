using System;
using System.Threading.Tasks;
using TGS.GameProviders.Repositories.Domains.Models.SettingConfiguration;
using TGS.GameProviders.Repositories.Entities;

namespace TGS.GameProviders.Core
{
    public interface ISexyGamingService
    {
        Task<GameProvider> GetGameProviderAsync();

        string GetGameProviderCode();

        ValueTask<string> GetGameProviderCultureCodeAsync(string cultureCode);

        ValueTask<string> GetGameProviderCurrencyCodeAsync(string currencyCode);

        Task<Guid> GetGameProviderIdAsync();

        Task<SexyGamingConfiguration> GetSettingConfigurationAsync();
    }
}
