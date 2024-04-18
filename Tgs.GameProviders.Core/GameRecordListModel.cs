using System;
using Tgs.GameRounds.Common;

namespace Tgs.GameProviders.Core;

public record GameRecordListModel
{
	public string GameProviderCode { get; init; }
	public Guid GameProviderId { get; init; }
	public string PlayerCode { get; init; }
	public GameRoundStatus Status { get; init; }
	public GameRoundClientStatus ClientStatus { get; init; }
	public string ExternalRoundId { get; init; }
	public string ExternalGameId { get; init; }
	public DateTimeOffset? ExternalCreatedOn { get; init; }
	public DateTimeOffset? ExternalClosedOn { get; init; }
	public decimal? BetAmount { get; init; }
	public decimal? ValidBetAmount { get; init; }
	public decimal? PlayerWinLoss { get; init; }
	public decimal? Commission { get; init; }
	public decimal? Turnover { get; init; }
	public decimal? SettleAmount { get; init; }
	public decimal? TieBetAdjustment { get; init; }
	public string AdditionalData { get; init; }
}
