using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tgs.GameProviders.Core;

public interface IGameBetRepository
{
	ValueTask<GameBetInfo> GetAsync(Guid gameBetId, CancellationToken cancellationToken = default);
}
