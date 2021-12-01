using System;
using System.Collections.Generic;
using TGS.GameProviders.Core.Repositories.Models.GameAction;

namespace TGS.GameProviders.Core.Repositories
{
    public interface IGameActionRepository
    {
        public IAsyncEnumerable<GameActionModel> GetGameActionsByRoundIdAsync(Guid roundId);
    }
}