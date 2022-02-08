using System;
using System.Threading.Tasks;
using TGS.GameProviders.Core.Repositories.Models.GameProvider;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Core.SBO
{
    public interface ISBOService
    {
        ValueTask<GameInfo> GetGameAsync(int value);

        ValueTask<GameProviderModel> GetGameProviderAsync(Guid gameProviderId);

        ValueTask<string> GetGameProviderCultureCodeAsync(Guid gameProviderId, string cultureCode);

        ValueTask<string> GetGameProviderCurrencyCodeAsync(Guid gameProviderId, string currencyCode);

        ValueTask<Guid> GetGameProviderIdAsync();

        ValueTask<SBOConfiguration> GetSettingConfigurationAsync();
    }
}
