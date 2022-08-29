using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Tgs.GameProviders.Client;

public interface IDreamGameClient
{
    ValueTask<GameRecordResponse?> GameRecordListAsync(CancellationToken cancellationToken = default);

    ValueTask MarkGameRecordListAsync(IEnumerable<string> externalRoundIds, CancellationToken cancellationToken = default);
}
