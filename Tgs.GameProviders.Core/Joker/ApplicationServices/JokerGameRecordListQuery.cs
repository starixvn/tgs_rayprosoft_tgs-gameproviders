#nullable enable

using System;

namespace Tgs.GameProviders.Core.Joker.ApplicationServices;

public record JokerGameRecordListQuery(
    string BrandCode,
    string AgentId,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime,
    string? NextId);