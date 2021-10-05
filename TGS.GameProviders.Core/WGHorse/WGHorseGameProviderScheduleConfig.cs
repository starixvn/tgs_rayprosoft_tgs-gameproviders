using System;

namespace TGS.GameProviders.Core.WGHorse
{
    public record WGHorseGameProviderScheduleConfig
    {
        public int IntervalDayMax { get; set; }
        public int IntervalDay { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }
        public DateTimeOffset LastGameRecordTime { get; set; }
    }
}
