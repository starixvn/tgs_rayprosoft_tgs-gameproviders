using System;
using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameProviders.Core.AsiaGaming.ApplicationServices;

public record AsiaGamingGameUrlQuery
{
    public Uri BackUrl { get; init; } = default!;
    public string BrandCode { get; init; } = default!;
    public string CurrencyCode { get; init; } = default!;
    public int? GameId { get; init; }
    public GameType GameType { get; init; }
    public string IPAddress { get; init; } = default!;
    public string Language { get; init; } = default!;
    public Guid LoginRecordId { get; init; }
    public PlatformType PlatformType { get; init; }
    public decimal PlayerBalance { get; init; }
    public string PlayerCode { get; init; } = default!;
}
