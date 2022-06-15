using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameProviders.Repositories
{
    internal class GameCacheRepository : IGameRepository
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IGameRepository _game;

        public GameCacheRepository(
            IMemoryCache memoryCache,
            IGameRepository game)
        {
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            _game = game ?? throw new ArgumentNullException(nameof(game));
        }

        public async ValueTask<GameModel?> GetAsync(int gameId)
        {
            if (!_memoryCache.TryGetValue(gameId, out GameModel? gameModel))
            {
                gameModel = await _game.GetAsync(gameId).ConfigureAwait(false);
                if (gameModel != null)
                {
                    _ = _memoryCache.Set(gameId, gameModel, TimeSpan.FromHours(1));
                }
            }
            return gameModel;
        }
        public IAsyncEnumerable<GameModel> GetGameListAsync(Guid gameProviderId, GameType gameType)
            => _game.GetGameListAsync(gameProviderId, gameType);
    }
}
