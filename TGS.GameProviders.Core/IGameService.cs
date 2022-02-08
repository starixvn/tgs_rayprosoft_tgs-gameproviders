using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TGS.GameProviders.Core
{
    public interface IGameService
    {
        public ValueTask<GameInfo?> GetAsync(int gameId);

        public ValueTask<GameInfo?> GetCacheAsync(int gameId);

        public ValueTask<GameInfo?> GetAsync(Guid gameProviderId, string externalGameId);

        public IAsyncEnumerable<GameInfo> GetGameListAsync(Guid gameProviderId);

        public IAsyncEnumerable<GameInfo> GetGameListAsync(Guid gameProviderId, GameType gameType);
    }
}
