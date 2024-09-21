using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameCommand.Common;

namespace Tgs.GameProviders.Core;

public interface IGameRepository
{
	ValueTask<GameInfo> GetAsync(
		int gameId,
		CancellationToken cancellationToken = default);

	ValueTask<GameInfo> GetByExternalIdAsync(
		Guid gameProviderId,
		string externalId,
		CancellationToken cancellationToken = default);

	IAsyncEnumerable<GameInfo> GetGameListAsync(
		Guid gameProviderId,
		GameType gameType,
		CancellationToken cancellationToken = default);
}
