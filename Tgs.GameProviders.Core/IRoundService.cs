using System;
using System.Threading.Tasks;

namespace Tgs.GameProviders.Core
{
    public interface IRoundService
    {
        ValueTask<RoundInfo?> GetAsync(Guid roundId);
    }
}
