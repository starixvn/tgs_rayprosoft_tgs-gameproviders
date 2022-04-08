using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tgs.GameProviders.Client
{
    public interface IDreamGameClient
    {
        ValueTask<GameRecordResponse?> GameRecordListAsync();

        ValueTask MarkGameRecordListAsync(IEnumerable<string> externalRoundIds);
    }
}
