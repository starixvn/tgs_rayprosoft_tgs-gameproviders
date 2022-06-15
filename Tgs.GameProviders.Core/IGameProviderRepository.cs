#nullable enable
using System;
using System.Threading.Tasks;
using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameProviders.Core
{
    public interface IGameProviderRepository
    {
        ValueTask<GameProviderModel> GetAsync(Guid gameProviderId);

        ValueTask<AwcConfigurationData> GetAwcConfigurationData(Guid gameProviderConfigurationId);

        ValueTask<(string DesktopEndpoint, string MobileEndpoint)> GetGameEndpointsAsync(Guid gameProviderConfigurationId);

        ValueTask<string?> GetGameProviderCultureCodeAsync(Guid gameProviderId, string cultureCode);

        ValueTask<string?> GetGameProviderCurrencyCodeAsync(Guid gameProviderId, string currencyCode);

        ValueTask<Guid> GetGameProviderIdAsync(string gameProviderCode);
    }
}
