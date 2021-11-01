using System;

namespace TGS.GameProviders.Repositories.Domains.Models.FailedGameRecordOperation
{
    public record GetFailedGameRecordOperation
    {
        public Guid Id { get; set; }

        public Guid GameProviderId { get; set; }

        public string? GameProviderCode { get; set; }

        public string ExternalRoundId { get; set; } = default!;

        public string PlayerCode { get; set; } = default!;

        public int GameId { get; set; }

        public string ExternalGameId { get; set; } = default!;

        public string Detail { get; set; } = default!;

        public FailedGameRecordOperationAutoStatus AutoStatus { get; set; }

        public FailedGameRecordOperationProcesStatus ProcesStatus { get; set; }

        public DateTimeOffset CreatedOn { get; set; }

        public DateTimeOffset UpdatedOn { get; set; }
    }
}
