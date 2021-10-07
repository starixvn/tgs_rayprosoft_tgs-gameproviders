using System;
using System.Threading.Tasks;
using TGS.GameProviders.GameRecord.Core.Processors;
using TGS.GameProviders.Repositories.Domains.Models.Rounds;

namespace TGS.GameProviders.GameRecord.Core
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
