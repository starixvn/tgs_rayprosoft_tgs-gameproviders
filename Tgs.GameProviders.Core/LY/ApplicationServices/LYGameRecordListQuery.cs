using System;

namespace Tgs.GameProviders.Core.LY.ApplicationServices
{
    public record LYGameRecordListQuery
    {
        public string BrandCode { get; init; }

        public string CurrencyCode { get; init; }

        public DateTimeOffset StartTime { get; init; }

        public DateTimeOffset EndTime { get; init; }
    }
}
