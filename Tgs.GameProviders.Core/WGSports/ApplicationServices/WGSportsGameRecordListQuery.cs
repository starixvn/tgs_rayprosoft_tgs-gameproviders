using System;

namespace Tgs.GameProviders.Core.WGSports.ApplicationServices;

public record WGSportsGameRecordListQuery
{
    public bool IsSettlement { get; init; }
    public DateTimeOffset StartTime { get; init; }
    public DateTimeOffset EndTime { get; init; }
}
