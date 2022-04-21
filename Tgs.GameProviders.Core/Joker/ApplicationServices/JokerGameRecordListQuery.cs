#nullable enable

using System;

namespace Tgs.GameProviders.Core.Joker.ApplicationServices;

public record JokerGameRecordListQuery(
    string BrandCode,
    string CurrencyCode,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime,
    string? NextId);