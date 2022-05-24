#nullable enable

using CsvHelper.Configuration.Attributes;

namespace Tgs.GameProviders.Core.PragmaticPlayTransfer.Models;

public class PragmaticPlayTransferGameRecordData
{
    [Name("extPlayerID")]
    public string PlayerCode { get; init; } = default!;

    [Name("gameID")]
    public string ExternalGameId { get; init; } = default!;

    [Name("playSessionID")]
    public string ExternalRoundId { get; init; } = default!;

    [Name("parentSessionID")]
    public string ParentExternalRoundId { get; init; } = default!;

    [Name("startDate")]
    public string StartDate { get; init; } = default!;

    [Name("endDate")]
    public string EndDate { get; init; } = default!;

    [Name("status")]
    public string Status { get; init; } = default!;

    [Name("type")]
    public string PlayType { get; init; } = default!;

    [Name("currency")]
    public string CurrencyCode { get; init; } = default!;

    [Name("bet")]
    public decimal BetAmount { get; init; }

    [Name("win")]
    public decimal WinAmount { get; init; }

    [Name("jackpot")]
    public decimal Jackpot { get; init; }

    [Name("roundDetails")]
    [Optional]
    public string? RoundDetails { get; init; }
}