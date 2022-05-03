using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Tgs.GameProviders.QueryServer.Client;

namespace Tgs.GameProviders.Core
{
    internal class PlayerService : IPlayerService
    {
        private readonly IGameProvidersQueryServerClient _gameProvidersQueryServerClient;
        private readonly IMemoryCache _memoryCache;

        public PlayerService(
            IGameProvidersQueryServerClient gameProvidersQueryServerClient,
            IMemoryCache memoryCache
            )
        {
            _gameProvidersQueryServerClient = gameProvidersQueryServerClient ?? throw new ArgumentNullException(nameof(gameProvidersQueryServerClient));
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        }

        public async Task<PlayerInfo> GetAsync(string playerCode)
        {
            var playerInfo = default(PlayerInfo);
            var data = await _gameProvidersQueryServerClient.PlayerGetAsync(playerCode).ConfigureAwait(false);

            if (data != null)
            {
                playerInfo = new PlayerInfo(
                     data.ResellerId,
                     data.ResellerCode,
                     data.LicenseeId,
                     data.LicenseeCode,
                     data.BrandId,
                     data.BrandCode,
                     data.PlayerCode,
                     data.Username,
                     data.DefaultWalletId,
                     data.DefaultWalletCurrencyCode,
                     data.Status
                     );
            }
            return playerInfo;
        }

        public async Task<PlayerInfo> GetCacheAsync(string playerCode)
        {
            if (_memoryCache.TryGetValue(playerCode, out PlayerInfo playerInfo)) return playerInfo;
            playerInfo = await GetAsync(playerCode).ConfigureAwait(false);

            if (playerInfo != null)
            {
                _ = _memoryCache.Set(playerCode, playerInfo, TimeSpan.FromHours(1));
            }
            return playerInfo;
        }
    }
}