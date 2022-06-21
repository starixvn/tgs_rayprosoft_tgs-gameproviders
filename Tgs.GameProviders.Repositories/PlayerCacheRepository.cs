using Microsoft.Extensions.Caching.Memory;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameProviders.Repositories
{
    internal class PlayerCacheRepository : IPlayerRepository
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IPlayerRepository _player;

        public PlayerCacheRepository(
            IMemoryCache memoryCache,
            IPlayerRepository player)
        {
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            _player = player ?? throw new ArgumentNullException(nameof(player));
        }

        public async Task<PlayerModel> GetAsync(string playerCode)
        {
            var key = $"GameProviders:PlayerCache:{playerCode}".ToLower();
            if (!_memoryCache.TryGetValue(key, out PlayerModel playerModel))
            {
                playerModel = await _player.GetAsync(playerCode).ConfigureAwait(false);

                _ = _memoryCache.Set(key, playerModel, TimeSpan.FromHours(1));
            }
            return playerModel;
        }
    }
}
