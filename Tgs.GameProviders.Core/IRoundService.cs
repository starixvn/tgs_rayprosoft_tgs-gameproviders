using System;
using System.Threading.Tasks;

#nullable enable

namespace Tgs.GameProviders.Core;

public interface IRoundService
{
    ValueTask<RoundInfo?> GetAsync(Guid roundId);
}
