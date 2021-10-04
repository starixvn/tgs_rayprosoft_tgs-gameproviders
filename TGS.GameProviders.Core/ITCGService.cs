using System;
using System.Threading.Tasks;
using TGS.GameProviders.Domains.SettingConfiguration;
using TGS.GameProviders.Domains.SettingConfiguration.TCG;
using TGS.GameProviders.Repositories.Entities;

namespace TGS.GameProviders.Core
{
    public interface ITCGService
    {
        Task<GameProvider> GetGameProviderAsync();

        string GetGameProviderCode();

        Task<Guid> GetGameProviderIdAsync();

        ValueTask<TCGMerchant> GetMerchantAsync(string currencyCode);

        ValueTask<TCGMerchant> GetMerchantByMerchantCodeAsync(string merchantCode);

        Task<TCGConfiguration> GetSettingConfigurationAsync();
    }
}
