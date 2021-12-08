using System;
using System.Threading.Tasks;
using TGS.GameProviders.Core.Repositories.Models.GameProvider;

namespace TGS.GameProviders.Core.Repositories
{
    public interface IGameProviderRepository
    {
        ValueTask<GameProviderModel?> GetAsync(string gameProviderCode);

        ValueTask<GameProviderModel?> GetAsync(Guid gameProviderId);

        ValueTask<(string DesktopEndpoint, string MobileEndpoint)> GetGameEndpointsAsync(Guid gameProviderConfigurationId);

        ValueTask<(bool IsCultureCode, string? GameProviderCultureCode)> GetGameProviderCultureCodeAsync(
                  Guid gameProviderId, string cultureCode);

        ValueTask<(bool IsCurrencyCode, string? GameProviderCurrencyCode)> GetGameProviderCurrencyCodeAsync(
          Guid gameProviderId, string currencyCode);

        ValueTask<Guid> GetGameProviderIdAsync(string gameProviderCode);
    }
}
