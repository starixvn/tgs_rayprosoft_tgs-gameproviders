using System;

namespace Tgs.GameProviders.ViewModels;

public class JokerGameRecordListRequest
{
    public string AgentId { get; init; } = default!;

    public string BrandCode { get; init; } = default!;

    public DateTimeOffset EndTime { get; init; }

    public string? NextId { get; init; }

    public DateTimeOffset StartTime { get; init; }
}
