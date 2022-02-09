using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGS.GameProviders.Core
{
    public interface IGameProviderService
    {

        ValueTask<GameProviderInfo> GetAsync(Guid gameProviderId);

        ValueTask<(string DesktopEndpoint, string MobileEndpoint)> GetGameEndpointsAsync(Guid gameProviderConfigurationId);

        ValueTask<string> GetGameProviderCultureCodeAsync(Guid gameProviderId, string cultureCode);

        ValueTask<string> GetGameProviderCurrencyCodeAsync(Guid gameProviderId, string currencyCode);

        ValueTask<Guid> GetGameProviderIdAsync(string gameProviderCode);

        ValueTask<Guid> GetCacheGameProviderIdAsync(string gameProviderCode);
    }
}
