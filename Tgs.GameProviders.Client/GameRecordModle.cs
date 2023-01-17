namespace Tgs.GameProviders.Client;

public record GameRecordModle
{
    public string GameProviderCode { get; init; } = default!;
    public Guid GameProviderId { get; init; }
    public string PlayerCode { get; init; } = default!;
    public GameRecordRoundStatus Status { get; init; }
    public GameRecordRoundClientStatus ClientStatus { get; init; }
    public string ExternalRoundId { get; init; } = default!;
    public string ExternalGameId { get; init; } = default!;
    public DateTimeOffset? ExternalCreatedOn { get; init; }
    public DateTimeOffset? ExternalClosedOn { get; init; }
    public decimal? BetAmount { get; init; }
    public decimal? ValidBetAmount { get; init; }
    public decimal? PlayerWinLoss { get; init; }
    public decimal? Commission { get; init; }
    public decimal? Turnover { get; init; }
    public decimal? SettleAmount { get; init; }
    public decimal? TieBetAdjustment { get; init; }
    public string AdditionalData { get; init; } = default!;
}
