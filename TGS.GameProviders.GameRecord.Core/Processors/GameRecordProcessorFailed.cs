using System;
using System.Threading.Tasks;
using TGS.GameProviders.Repositories;
using TGS.GameProviders.Repositories.Domains;
using TGS.GameProviders.Repositories.Domains.Models.FailedGameRecordOperation;

namespace TGS.GameProviders.GameRecord.Core.Processors
{
    internal class GameRecordProcessorFailed : IGameRecordProcessorFailed
    {
        private readonly IFailedGameRecordOperationRepository _failedGameRecordOperationRepository;

        public GameRecordProcessorFailed(IFailedGameRecordOperationRepository failedGameRecordOperationRepository)
        {
            _failedGameRecordOperationRepository = failedGameRecordOperationRepository ?? throw new ArgumentNullException(nameof(failedGameRecordOperationRepository));
        }

        public async Task ProcessorAsync(GameRoundsData data)
        {
            var failed = await _failedGameRecordOperationRepository.GetFailedGameRecordOperationAsync(
                data.GameProviderId,
                data.ExternalRoundId,
                data.GameId,
                data.PlayerCode).ConfigureAwait(false);

            if (failed == null)
            {
                await FailedInsertAsync(data).ConfigureAwait(false);
            }
            else
            {
                if (failed.ProcesStatus == FailedGameRecordOperationProcesStatus.Process)
                {
                    await FailedUpdateAsync(failed, data).ConfigureAwait(false);
                }
            }
        }

        private Task FailedInsertAsync(GameRoundsData data)
           => _failedGameRecordOperationRepository.AddFailedGameRecordOperationAsync(new AddFailedGameRecordOperation(
               GameProviderId: data.GameProviderId,
               GameProviderCode: data.GameProviderCode,
               ExternalRoundId: data.ExternalRoundId,
               PlayerCode: data.PlayerCode,
               GameId: data.GameId,
               ExternalGameId: data.ExternalGameId,
               Detail: data.AdditionalData,
               AutoStatus: FailedGameRecordOperationAutoStatus.Manual,
               ProcesStatus: FailedGameRecordOperationProcesStatus.Process,
               CreatedOn: DateTimeOffset.UtcNow,
               UpdatedOn: DateTimeOffset.UtcNow
          ));

        private Task FailedUpdateAsync(GetFailedGameRecordOperation failed, GameRoundsData data)
          => _failedGameRecordOperationRepository.UpdataFailedGameRecordOperationAsync(failed.Id, data.AdditionalData);
    }
}
