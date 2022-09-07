using Tgs.GameProviders.Core;
using Tgs.GameProviders.QueryServer.Client;
using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameProviders.Repositories;

internal class GameRepository : IGameRepository
{
    private readonly IGameProvidersQueryServerClient _client;

    public GameRepository(IGameProvidersQueryServerClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public ValueTask<GameModel?> GetAsync(int gameId)
        => _client.GameGetAsync(gameId);

    public IAsyncEnumerable<GameModel> GetGameListAsync(Guid gameProviderId, GameType gameType)
        => _client.GameQueryAsync(gameProviderId, gameType);
}
