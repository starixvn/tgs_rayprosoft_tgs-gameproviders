using Tgs.GameCommand.Client;
using Tgs.GameCommand.Common;
using Tgs.GameProviders.Core;

namespace Tgs.GameProviders.Repositories;

internal class GameRepository(IGameCommandClient client) : IGameRepository
{
	public async ValueTask<GameInfo> GetAsync(int gameId, CancellationToken cancellationToken = default)
		=> await client.GameGetAsync(
			gameId: gameId,
			cancellationToken: cancellationToken)
		.ConfigureAwait(false)
		?? throw new ArgumentNullException(gameId.ToString(), $"This game does not exist.");

	public async ValueTask<GameInfo> GetByExternalIdAsync(Guid gameProviderId, string externalId, CancellationToken cancellationToken = default)
	{
		var gameInfo = await client.GameGetAsync(gameProviderId, externalId, cancellationToken).ConfigureAwait(false);

		return gameInfo ?? await GetByExternalIdAsync(gameProviderId, "0", cancellationToken).ConfigureAwait(false);
	}

	public IAsyncEnumerable<GameInfo> GetGameListAsync(Guid gameProviderId, GameType gameType, CancellationToken cancellationToken = default)
		=> client.GameQueryAsync(
			gameProviderId: gameProviderId,
			gameType: gameType,
			cancellationToken: cancellationToken);
}
