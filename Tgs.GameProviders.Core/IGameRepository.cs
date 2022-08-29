#nullable enable

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameProviders.Core;

public interface IGameRepository
{
    ValueTask<GameModel?> GetAsync(int gameId);

    IAsyncEnumerable<GameModel> GetGameListAsync(Guid gameProviderId, GameType gameType);
}
