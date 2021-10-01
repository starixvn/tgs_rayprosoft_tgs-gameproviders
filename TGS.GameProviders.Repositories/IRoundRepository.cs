using System;
using System.Threading.Tasks;
using TGS.GameProviders.Repositories.Domains.Models.Rounds;

namespace TGS.GameProviders.Repositories
{
    public interface IRoundRepository
    {
        Task AddRoundAsync(AddRound source);

        Task<GetRound?> GetRoundAsync(Guid RoundId);

        Task<GetRound?> GetRoundAsync(Guid gameProviderId, string externalRoundId, int gameId);

        Task<GetRound?> GetRoundAsync(Guid gameProviderId, string externalRoundId, int gameId, string playerCode);

        Task UpdateRoundAsync(UpdateRound source);

        Task UpdateRoundByExternalRoundIdAsync(UpdateRoundByExternalRoundId source);

        Task UpdateRoundDetailAsync(Guid roundId, string additionalData);
    }
}
