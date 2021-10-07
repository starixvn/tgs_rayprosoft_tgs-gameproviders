using System;

namespace TGS.GameProviders.Core.BigGaming
{
    public record BigGamingGameProviderScheduleConfig
    {
        public decimal IntervalMinuteMax { get; set; }
        public decimal IntervalMinute { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }
        public DateTimeOffset LastGameRecordTime { get; set; }
    }
}
