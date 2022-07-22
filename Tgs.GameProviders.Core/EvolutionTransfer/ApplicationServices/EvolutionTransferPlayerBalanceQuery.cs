using System;

namespace Tgs.GameProviders.Core.EvolutionTransfer.ApplicationServices
{
    public record EvolutionTransferPlayerBalanceQuery : PlayerBalanceQuery
    {
        public int GameId { get; init; } = default!;
        public string CurrencyCode { get; init; } = default!;
        public Guid LoginRecordId { get; init; } = default!;
        public PlatformType PlatformType { get; init; }
        public string Language { get; init; } = default!;
        public string IPAddress { get; init; } = default!;
    }
}
