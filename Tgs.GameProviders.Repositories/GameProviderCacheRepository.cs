using Microsoft.Extensions.Caching.Memory;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameProviders.Repositories
{
    internal class GameProviderCacheRepository : IGameProviderRepository
    {
        private readonly IGameProviderRepository _gameProvider;
        private readonly IMemoryCache _memoryCache;

        public GameProviderCacheRepository(
            IMemoryCache memoryCache,
            IGameProviderRepository gameProvider)
        {
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
        }

        public ValueTask<GameProviderModel> GetAsync(Guid gameProviderId)
             => _gameProvider.GetAsync(gameProviderId);

        public async ValueTask<AwcConfigurationData> GetAwcConfigurationData(Guid gameProviderConfigurationId)
        {
            if (!_memoryCache.TryGetValue(gameProviderConfigurationId, out AwcConfigurationData configurationData))
            {
                configurationData = await _gameProvider.GetAwcConfigurationData(gameProviderConfigurationId).ConfigureAwait(false);

                _ = _memoryCache.Set(gameProviderConfigurationId, configurationData, TimeSpan.FromHours(1));
            }
            return configurationData;
        }

        public ValueTask<(string DesktopEndpoint, string MobileEndpoint)> GetGameEndpointsAsync(Guid gameProviderConfigurationId)
            => _gameProvider.GetGameEndpointsAsync(gameProviderConfigurationId);

        public ValueTask<string?> GetGameProviderCultureCodeAsync(Guid gameProviderId, string cultureCode)
            => _gameProvider.GetGameProviderCultureCodeAsync(gameProviderId, cultureCode);

        public ValueTask<string?> GetGameProviderCurrencyCodeAsync(Guid gameProviderId, string currencyCode)
            => _gameProvider.GetGameProviderCurrencyCodeAsync(gameProviderId, currencyCode);

        public async ValueTask<Guid> GetGameProviderIdAsync(string gameProviderCode)
        {
            if (!_memoryCache.TryGetValue(gameProviderCode, out Guid gameProviderId))
            {
                gameProviderId = await _gameProvider.GetGameProviderIdAsync(gameProviderCode).ConfigureAwait(false);
                _ = _memoryCache.Set(gameProviderCode, gameProviderId, TimeSpan.FromDays(1));
            }
            return gameProviderId;
        }
    }
}
