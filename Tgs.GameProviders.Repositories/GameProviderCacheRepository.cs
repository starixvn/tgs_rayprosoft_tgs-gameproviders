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
            var key = $"GameProviders:GameProviderCache:{gameProviderConfigurationId}".ToLower();
            if (!_memoryCache.TryGetValue(key, out AwcConfigurationData configurationData))
            {
                configurationData = await _gameProvider.GetAwcConfigurationData(gameProviderConfigurationId).ConfigureAwait(false);

                _ = _memoryCache.Set(key, configurationData, TimeSpan.FromMinutes(1));
            }
            return configurationData;
        }

        public ValueTask<(string DesktopEndpoint, string MobileEndpoint)> GetGameEndpointsAsync(Guid gameProviderConfigurationId)
            => _gameProvider.GetGameEndpointsAsync(gameProviderConfigurationId);

        public async ValueTask<string?> GetGameProviderCultureCodeAsync(Guid gameProviderId, string cultureCode)
        {
            var key = $"GameProviders:GameProviderCache:{gameProviderId}:{cultureCode}".ToLower();
            if (!_memoryCache.TryGetValue(key, out string? gameProviderCultureCode))
            {
                gameProviderCultureCode = await _gameProvider.GetGameProviderCultureCodeAsync(gameProviderId, cultureCode).ConfigureAwait(false);

                if (!string.IsNullOrWhiteSpace(gameProviderCultureCode))
                {
                    _ = _memoryCache.Set(key, gameProviderCultureCode, TimeSpan.FromHours(1));
                }
            }
            return gameProviderCultureCode;
        }

        public async ValueTask<string?> GetGameProviderCurrencyCodeAsync(Guid gameProviderId, string currencyCode)
        {
            var key = $"GameProviders:GameProviderCache:{gameProviderId}:{currencyCode}".ToLower();
            if (!_memoryCache.TryGetValue(key, out string? gameProviderCurrencyCode))
            {
                gameProviderCurrencyCode = await _gameProvider.GetGameProviderCultureCodeAsync(gameProviderId, currencyCode).ConfigureAwait(false);

                if (!string.IsNullOrWhiteSpace(gameProviderCurrencyCode))
                {
                    _ = _memoryCache.Set(key, gameProviderCurrencyCode, TimeSpan.FromHours(1));
                }
            }
            return gameProviderCurrencyCode;
        }

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
