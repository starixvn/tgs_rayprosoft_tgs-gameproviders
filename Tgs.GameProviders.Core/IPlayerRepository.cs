using System.Threading;
using System.Threading.Tasks;
using Tgs.GameCommand.Common;

namespace Tgs.GameProviders.Core;

public interface IPlayerRepository
{
    ValueTask<PlayerInfo> GetAsync(
        string playerCode,
        CancellationToken cancellationToken = default);
}
