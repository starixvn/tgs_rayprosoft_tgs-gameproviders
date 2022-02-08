using System;
using System.Collections.Generic;

namespace TGS.GameProviders.Core
{
    public interface IGameActionService
    {
        public IAsyncEnumerable<GameActionInfo> QueryAsync(Guid roundId);
    }
}
