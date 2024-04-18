using Tgs.GameCommand.Client;
using Tgs.GameCommand.Common;
using Tgs.GameProviders.Core;

namespace Tgs.GameProviders.Repositories;

public class PlayerRepository(IGameCommandClient client) : IPlayerRepository
{
	public async ValueTask<PlayerInfo> GetAsync(string playerCode, CancellationToken cancellationToken = default)
		=> await client.PlayerGetAsync(
			playerCode: playerCode,
			cancellationToken: cancellationToken)
		.ConfigureAwait(false)
		?? throw new ArgumentNullException(playerCode, "The player does not exist.");
}
