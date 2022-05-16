using CsvHelper.Configuration.Attributes;

namespace Tgs.GameProviders.Core.PragmaticPlayTransfer.Models;

internal class PragmaticPlayTransferGameRecordData
{
    [Name("extPlayerID")]
    public string PlayerCode { get; init; }

    [Name("gameID")]
    public string ExternalGameId { get; init; }

    [Name("playSessionID")]
    public string ExternalRoundId { get; init; }

    [Name("parentSessionID")]
    public string ParentExternalRoundId { get; init; }

    [Name("startDate")]
    public string StartDate { get; init; }

    [Name("endDate")]
    public string EndDate { get; init; }

    [Name("status")]
    public string Status { get; init; }

    [Name("type")]
    public string PlayType { get; init; }

    [Name("currency")]
    public string CurrencyCode { get; init; }

    [Name("bet")]
    public decimal BetAmount { get; init; }

    [Name("win")]
    public decimal WinAmount { get; init; }

    [Name("jackpot")]
    public decimal Jackpot { get; init; }
}