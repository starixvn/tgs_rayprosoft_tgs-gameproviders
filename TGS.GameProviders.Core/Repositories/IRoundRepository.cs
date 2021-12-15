using System;
using System.Threading.Tasks;
using TGS.GameProviders.Core.Repositories.Models.Rounds;

namespace TGS.GameProviders.Core.Repositories
{
    public interface IRoundRepository
    {
        Task<RoundModel?> GetAsync(Guid roundId);

        Task<RoundModel?> GetAsync(Guid gameProviderId, string externalRoundId, int gameId);

        Task<RoundModel?> GetAsync(Guid gameProviderId, string externalRoundId, int gameId, string playerCode);
    }
}
