using System;

namespace Tgs.GameProviders.Core.BigGaming.ApplicationServices;

public record BigGamingFishingGameRecordListQuery
{
    public string CurrencyCode { get; set; } = default!;
    public DateTimeOffset StartTime { get; init; }
    public DateTimeOffset EndTime { get; init; }
    public int Page { get; init; }
    public int Rows { get; init; }
}
