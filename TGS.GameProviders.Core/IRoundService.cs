using System;
using System.Threading.Tasks;

namespace TGS.GameProviders.Core
{
    public interface IRoundService
    {
        ValueTask<RoundInfo?> GetAsync(Guid roundId);
    }
}
