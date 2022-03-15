using System.Collections.Generic;
using System.Threading.Tasks;

namespace TGS.GameProviders.Client
{
    public interface IDreamGameClient
    {
        ValueTask<GameRecordResponse?> GameRecordListAsync();

        ValueTask MarkGameRecordListAsync(IEnumerable<string> externalRoundIds);
    }
}
