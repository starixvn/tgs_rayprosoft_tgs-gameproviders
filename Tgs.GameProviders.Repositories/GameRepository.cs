using Tgs.GameCommand.Client;
using Tgs.GameCommand.Common;
using Tgs.GameProviders.Core;

namespace Tgs.GameProviders.Repositories;

internal class GameRepository(IGameCommandClient client) : IGameRepository
{
    public async ValueTask<GameInfo> GetAsync(
        int gameId,
        CancellationToken cancellationToken = default)
        => await client.GameGetAsync(
            gameId: gameId,
            cancellationToken: cancellationToken)
        .ConfigureAwait(false)
        ?? throw new Exception($"This game does not exist.{gameId}");

    public IAsyncEnumerable<GameInfo> GetGameListAsync(
        Guid gameProviderId,
        GameType gameType,
        CancellationToken cancellationToken = default)
        => client.GameQueryAsync(
            gameProviderId: gameProviderId,
            gameType: gameType,
            cancellationToken: cancellationToken);
}
