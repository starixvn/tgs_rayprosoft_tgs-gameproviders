using Tgs.GameCommand.Client;
using Tgs.GameCommand.Common;
using Tgs.GameProviders.Core;

namespace Tgs.GameProviders.Repositories;

internal class GameRepository : IGameRepository
{
    private readonly IGameCommandClient _client;

    public GameRepository(IGameCommandClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask<GameInfo> GetAsync(
        int gameId,
        CancellationToken cancellationToken = default)
        => await _client
        .GameGetAsync(
            gameId: gameId,
            cancellationToken: cancellationToken)
        .ConfigureAwait(false)
        ?? throw new Exception($"This game does not exist.{gameId}");

    public IAsyncEnumerable<GameInfo> GetGameListAsync(
        Guid gameProviderId,
        GameType gameType,
        CancellationToken cancellationToken = default)
        => _client.GameQueryAsync(
            gameProviderId: gameProviderId,
            gameType: gameType,
            cancellationToken: cancellationToken);
}
