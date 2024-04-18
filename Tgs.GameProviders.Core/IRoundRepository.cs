using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tgs.GameProviders.Core;

public interface IRoundRepository
{
	ValueTask<RoundInfo> GetAsync(
		Guid roundId,
		CancellationToken cancellationToken = default);
}
