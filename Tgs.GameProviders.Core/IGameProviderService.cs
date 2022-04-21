using System;
using System.Threading.Tasks;

namespace Tgs.GameProviders.Core
{
    public interface IGameProviderService
    {
        ValueTask<GameProviderInfo> GetAsync(Guid gameProviderId);

        ValueTask<Guid> GetCacheGameProviderIdAsync(string gameProviderCode);

        ValueTask<(string DesktopEndpoint, string MobileEndpoint)> GetGameEndpointsAsync(Guid gameProviderConfigurationId);

        ValueTask<string> GetGameProviderCultureCodeAsync(Guid gameProviderId, string cultureCode);

        ValueTask<string> GetGameProviderCurrencyCodeAsync(Guid gameProviderId, string currencyCode);

        ValueTask<Guid> GetGameProviderIdAsync(string gameProviderCode);
    }
}
