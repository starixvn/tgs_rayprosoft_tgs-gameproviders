using System;

namespace Tgs.GameProviders.Core.WGHorse.ApplicationServices;

public record WGHorseGameRecordListQuery
{
    public DateTimeOffset StartTime { get; init; }
    public DateTimeOffset EndTime { get; init; }
    public int Page { get; init; }
    public int Rows { get; init; }
}
