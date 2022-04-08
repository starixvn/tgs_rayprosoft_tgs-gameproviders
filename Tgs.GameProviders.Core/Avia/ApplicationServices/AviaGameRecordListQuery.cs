using System;

namespace Tgs.GameProviders.Core.Avia.ApplicationServices
{
    public record AviaGameRecordListQuery
    {
        public string BrandCode { get; init; }
        public DateTimeOffset StartTime { get; init; }
        public DateTimeOffset EndTime { get; init; }
        public int Page { get; init; }
        public int Rows { get; init; }
    }
}
