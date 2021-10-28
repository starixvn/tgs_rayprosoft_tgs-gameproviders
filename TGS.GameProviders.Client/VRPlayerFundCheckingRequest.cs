using System;

namespace TGS.GameProviders.Client
{
    public record VRPlayerFundCheckingRequest
    {
        public string BrandCode { get; init; } = default!;

        public string PlayerCode { get; init; } = default!;

        public string CurrencyCode { get; init; } = default!;

        public string OrderId { get; set; } = default!;

        public DateTimeOffset StartTime { get; init; } = default!;

        public DateTimeOffset EndTime { get; init; } = default!;
    }
}
