using System;
using System.Threading.Tasks;
using TGS.GameProviders.Repositories.Domains.Models.Rounds;

namespace TGS.GameProviders.Core.GameRecordProcessor
{
    public interface IGameRecordProcessor
    {
        ValueTask<bool> ExecuteAsync(GameRoundsData data);

        Task<GetRound> GetAsync(Guid roundId);

        Task<GetRound> GetExternalRoundIdAsync(Guid gameProviderId, string externalRoundId, int gameId, string playerCode);

        Task<GetRound> GetExternalRoundIdByExternalGameIdAsync(Guid gameProviderId, string externalRoundId, string externalGameId, string playerCode);

        Task UpdateRoundDetailAsync(Guid roundId, string additionalData);
    }
}
