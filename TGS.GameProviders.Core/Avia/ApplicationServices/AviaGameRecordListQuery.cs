using System;

namespace TGS.GameProviders.Core.Avia.ApplicationServices
{
    public record AviaGameRecordListQuery
    {
        public DateTimeOffset StartTime { get; init; }
        public DateTimeOffset EndTime { get; init; }
        public int Page { get; init; }
        public int Rows { get; init; }
    }
}
