using System;
using System.Collections.Generic;

namespace Tgs.GameProviders.Core
{
    public interface IGameActionService
    {
        public IAsyncEnumerable<GameActionInfo> QueryAsync(Guid roundId);
    }
}
