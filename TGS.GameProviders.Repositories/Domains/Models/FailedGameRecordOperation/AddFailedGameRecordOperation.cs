using System;

namespace TGS.GameProviders.Repositories.Domains.Models.FailedGameRecordOperation
{
    public record AddFailedGameRecordOperation(
       Guid GameProviderId,
       string GameProviderCode,
       string ExternalRoundId,
       string PlayerCode,
       int GameId,
       string ExternalId,
       string Detail,
       FailedGameRecordOperationAutoStatus AutoStatus,
       FailedGameRecordOperationProcesStatus ProcesStatus,
       DateTimeOffset CreatedOn,
       DateTimeOffset UpdatedOn
        );
}
