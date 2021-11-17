using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TGS.GameProviders.Core.Repositories.Models.Game;
using TGS.GameProviders.Domains;

namespace TGS.GameProviders.Core.Repositories
{
    public interface IGameRepository
    {
        public Task<GameModel> GetAsync(int gameId);

        public Task<GameModel> GetAsync(Guid gameProviderId, string externalGameId);

        public IAsyncEnumerable<GameModel> GetGameList(Guid gameProviderId);

        public IAsyncEnumerable<GameModel> GetGameList(Guid gameProviderId, GameType gameType);
    }
}
