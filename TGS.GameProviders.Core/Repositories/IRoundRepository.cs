using System;
using System.Threading.Tasks;
using TGS.GameProviders.Core.Repositories.Models.Rounds;

namespace TGS.GameProviders.Core.Repositories
{
    public interface IRoundRepository
    {
        Task<GetRound?> GetRoundAsync(Guid RoundId);

        Task<GetRound?> GetRoundAsync(Guid gameProviderId, string externalRoundId, int gameId);

        Task<GetRound?> GetRoundAsync(Guid gameProviderId, string externalRoundId, int gameId, string playerCode);
    }
}
