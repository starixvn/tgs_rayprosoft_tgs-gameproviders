using System;

namespace Tgs.GameProviders.Core.KY.ApplicationServices;

public record KYGameRecordListQuery
{
    public string BrandCode { get; init; }

    public string CurrencyCode { get; init; }

    public DateTimeOffset StartTime { get; init; }

    public DateTimeOffset EndTime { get; init; }
}
