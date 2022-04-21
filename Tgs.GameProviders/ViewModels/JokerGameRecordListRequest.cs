using System;

namespace Tgs.GameProviders.ViewModels;

public class JokerGameRecordListRequest
{
    public string BrandCode { get; init; } = default!;

    public string CurrencyCode { get; init; } = default!;

    public DateTimeOffset StartTime { get; init; }

    public DateTimeOffset EndTime { get; init; }

    public string? NextId { get; init; }
}