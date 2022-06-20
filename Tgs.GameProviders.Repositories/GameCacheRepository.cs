using Microsoft.Extensions.Caching.Memory;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameProviders.Repositories
{
    internal class GameCacheRepository : IGameRepository
    {
        private readonly IGameRepository _game;
        private readonly IMemoryCache _memoryCache;

        public GameCacheRepository(
            IMemoryCache memoryCache,
            IGameRepository game)
        {
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            _game = game ?? throw new ArgumentNullException(nameof(game));
        }

        public async ValueTask<GameModel?> GetAsync(int gameId)
        {
            var key = $"GameProviders:GameCache:{gameId}".ToLower();
            if (!_memoryCache.TryGetValue(key, out GameModel? gameModel))
            {
                gameModel = await _game.GetAsync(gameId).ConfigureAwait(false);
                if (gameModel is not null)
                {
                    _ = _memoryCache.Set(key, gameModel, TimeSpan.FromHours(1));
                }
            }
            return gameModel;
        }

        public IAsyncEnumerable<GameModel> GetGameListAsync(Guid gameProviderId, GameType gameType)
            => _game.GetGameListAsync(gameProviderId, gameType);
    }
}
