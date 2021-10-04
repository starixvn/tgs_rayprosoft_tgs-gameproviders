using System;
using System.Threading.Tasks;
using TGS.GameProviders.Domains.SettingConfiguration;
using TGS.GameProviders.Domains.SettingConfiguration.BigGaming;
using TGS.GameProviders.Repositories.Entities;

namespace TGS.GameProviders.Core
{
    public interface IBigGamingService
    {
        ValueTask<BigGamingBrandMerchant> GetBrandMerchantAsync(string currencyCode);

        Task<GameProvider> GetGameProviderAsync();

        string GetGameProviderCode();

        Task<Guid> GetGameProviderIdAsync();

        Task<BigGamingConfiguration> GetSettingConfigurationAsync();
    }
}
