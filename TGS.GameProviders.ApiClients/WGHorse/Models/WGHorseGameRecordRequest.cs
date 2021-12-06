using System;

namespace TGS.GameProviders.ApiClients.WGHorse.Models
{
    public record WGHorseGameRecordRequest
    {
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
