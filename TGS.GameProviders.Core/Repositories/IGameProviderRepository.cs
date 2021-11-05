using System;
using System.Threading.Tasks;

namespace TGS.GameProviders.Core.Repositories
{
    public interface IGameProviderRepository
    {
        ValueTask<(bool IsCultureCode, string? GameProviderCultureCode)> GetGameProviderCultureCodeAsync(Guid gameProviderId, string cultureCode);

        ValueTask<(bool IsCurrencyCode, string? GameProviderCurrencyCode)> GetGameProviderCurrencyCodeAsync(Guid gameProviderId, string currencyCode);

        ValueTask<Guid> GetGameProviderIdAsync(string gameProviderCode);

        ValueTask<(string DesktopEndpoint, string MobileEndpoint)> GetGameEndpointsAsync(Guid gameProviderConfigurationId);
    }
}