using System;

namespace Tgs.GameProviders.Core.VR.ApplicationServices;

public record VRGameRecordListQuery
{
    public string BrandCode { get; init; }
    public DateTimeOffset StartTime { get; init; }
    public DateTimeOffset EndTime { get; init; }
    public bool IsUpdateTime { get; init; }
    public int Page { get; init; }
    public int Rows { get; init; }
}
