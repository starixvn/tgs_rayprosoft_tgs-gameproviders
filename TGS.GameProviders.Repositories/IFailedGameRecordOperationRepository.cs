using System;
using System.Threading.Tasks;
using TGS.GameProviders.Repositories.Domains.Models.FailedGameRecordOperation;

namespace TGS.GameProviders.Repositories
{
    public interface IFailedGameRecordOperationRepository
    {
        Task AddFailedGameRecordOperationAsync(AddFailedGameRecordOperation source);

        Task<GetFailedGameRecordOperation> GetFailedGameRecordOperationAsync(Guid gameProviderId, string externalRoundId, int gameId, string playerCode);

        Task UpdataFailedGameRecordOperationAsync(Guid failedGameRecordOperationId, string detail);
    }
}
