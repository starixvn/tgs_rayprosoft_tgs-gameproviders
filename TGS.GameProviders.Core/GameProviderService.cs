using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Tgs.GameProviders.QueryServer.Client;

namespace TGS.GameProviders.Core
{
    internal class GameProviderService : IGameProviderService
    {
        private readonly IGameProvidersQueryServerClient _gameProvidersQueryServerClient;
        private readonly IMemoryCache _memoryCache;

        public GameProviderService(
            IGameProvidersQueryServerClient gameProvidersQueryServerClient,
            IMemoryCache memoryCache
            )
        {
            _gameProvidersQueryServerClient = gameProvidersQueryServerClient ?? throw new ArgumentNullException(nameof(gameProvidersQueryServerClient));
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        }


        public async ValueTask<GameProviderInfo> GetAsync(Guid gameProviderId)
        {
            var data = await _gameProvidersQueryServerClient.GameProviderGetAsync(gameProviderId).ConfigureAwait(false);
            return data == null
                ? throw new Exception($"Game Provider does not exist.({gameProviderId})")
                : new GameProviderInfo(
                data.GameProviderId,
                data.Status,
                data.Description,
                data.AutoCloseRound,
                data.SupportTestPlayer,
                data.AllowKickPlayer,
                data.PlaceBetTimeout,
                data.SettleBetTimeout,
                data.UniqueId,
                data.TransferWallet,
                data.GameProviderCode,
                data.Name,
                data.Localization);
        }
        public async ValueTask<(string DesktopEndpoint, string MobileEndpoint)> GetGameEndpointsAsync(Guid gameProviderConfigurationId)
        {
            var data = await _gameProvidersQueryServerClient.GameProviderGetGameProviderConfigAsync(gameProviderConfigurationId).ConfigureAwait(false);
            return data == null
                ? throw new Exception($"Game Provider Configuration does not exist.({gameProviderConfigurationId})")
                : (new(data.DesktopEndpoint, data.MobileEndpoint));
        }
        public ValueTask<string> GetGameProviderCultureCodeAsync(Guid gameProviderId, string cultureCode)
            => _gameProvidersQueryServerClient.GameProviderGetCultureCodeAsync(gameProviderId, cultureCode);

        public ValueTask<string> GetGameProviderCurrencyCodeAsync(Guid gameProviderId, string currencyCode)
            => _gameProvidersQueryServerClient.GameProviderGetCurrencyCodeAsync(gameProviderId, currencyCode);

        public async ValueTask<Guid> GetGameProviderIdAsync(string gameProviderCode)
        {
            var gameProviderId = await _gameProvidersQueryServerClient.GameProviderGetGameProviderIdAsync(gameProviderCode).ConfigureAwait(false);
            return !gameProviderId.HasValue ? throw new Exception($"Game Provider does not exist.({gameProviderCode})") : gameProviderId.Value;
        }

        public async ValueTask<Guid> GetCacheGameProviderIdAsync(string gameProviderCode)
        {
            if (!_memoryCache.TryGetValue(gameProviderCode, out Guid gameProviderId))
            {
                gameProviderId = await GetGameProviderIdAsync(gameProviderCode).ConfigureAwait(false);
                _memoryCache.Set(gameProviderCode, gameProviderId, TimeSpan.FromDays(1));
            }
            return gameProviderId;
        }
    }
}

