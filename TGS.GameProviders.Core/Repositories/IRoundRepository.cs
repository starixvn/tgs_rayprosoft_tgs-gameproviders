using System;
using System.Threading.Tasks;
using TGS.GameProviders.Core.Repositories.Models.Rounds;

namespace TGS.GameProviders.Core.Repositories
{
    public interface IRoundRepository
    {
        ValueTask<RoundModel?> GetRoundAsync(Guid RoundId);

        ValueTask<string?> GetRoundDetails(Guid roundId);

        ValueTask<RoundModel?> GetRoundAsync(Guid gameProviderId, string externalRoundId, int gameId);

        ValueTask<RoundModel?> GetRoundAsync(Guid gameProviderId, string externalRoundId, int gameId, string playerCode);
    }
}
