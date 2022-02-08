using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Tgs.GameProviders.QueryServer.Client;

namespace TGS.GameProviders.Core
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
            var playerViewModel = await _gameProvidersQueryServerClient.PlayerGetAsync(playerCode).ConfigureAwait(false);

            if (playerViewModel != null)
            {
                playerInfo = new PlayerInfo(
                     playerViewModel.ResellerId,
                     playerViewModel.ResellerCode,
                     playerViewModel.LicenseeId,
                     playerViewModel.LicenseeCode,
                     playerViewModel.BrandId,
                     playerViewModel.BrandCode,
                     playerViewModel.PlayerCode,
                     playerViewModel.Username,
                     playerViewModel.DefaultWalletId,
                     playerViewModel.DefaultWalletCurrencyCode,
                    (PlayerStatus)playerViewModel.Status
                     );
            }
            return playerInfo;
        }

        public async Task<PlayerInfo> GetCacheAsync(string playerCode)
        {
            if (!_memoryCache.TryGetValue(playerCode, out PlayerInfo playerInfo))
            {
                playerInfo = await GetAsync(playerCode).ConfigureAwait(false);

                if (playerInfo != null)
                {
                    _memoryCache.Set(playerCode, playerInfo, TimeSpan.FromHours(1));
                }
            }
            return playerInfo;
        }
    }
}
