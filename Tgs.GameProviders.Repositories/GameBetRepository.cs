using Microsoft.Extensions.Logging;
using Polly;
using Tgs.GameBets.Client;
using Tgs.GameProviders.Core;

namespace Tgs.GameProviders.Repositories;

internal class GameBetRepository(
	ILogger<GameBetRepository> logger,
	IGameBetsClient client) : IGameBetRepository
{
	public async ValueTask<GameBetInfo> GetAsync(Guid gameBetId, CancellationToken cancellationToken = default)
	{
		var retryCount = 5;
		var retrySeconds = 2;
		var retryPolicy = Policy
			.Handle<HttpRequestException>()
			.WaitAndRetryAsync(
				retryCount: retryCount,
				retryAttempt => TimeSpan.FromSeconds(Math.Pow(retrySeconds, retryAttempt)),
				onRetry: (exception, retryTime, retryCount, context) => logger.LogWarning(exception, "LogOn:{logOn} Exception:{exception} RetryTime:{retryTime} RetryCount:{retryCount} Source:{source}",
					DateTime.Now.ToString("o"),
					exception.Message,
					retryTime,
					retryCount,
					new
					{
						GameBetId = gameBetId
					}));

		var model = await retryPolicy.ExecuteAsync(() => client.GameBetGetAsync(
			id: gameBetId,
			externalCreatedOn: DateTimeOffset.Now.AddDays(-45),
			cancellationToken: cancellationToken).AsTask())
			.ConfigureAwait(false)
			?? throw new ArgumentNullException(gameBetId.ToString(), "This game bet does not exist.");

		return new GameBetInfo(
			Id: model.Id,
			GameProviderId: model.GameProviderId,
			GameProviderConfigurationId: model.GameProviderConfigurationId,
			GameId: model.GameId,
			ResellerId: model.ResellerId,
			LicenseeId: model.LicenseeId,
			BrandId: model.BrandId,
			PlayerCode: model.PlayerCode,
			CurrencyCode: model.CurrencyCode,
			WalletId: model.WalletId,
			ExternalRoundId: model.ExternalRoundId,
			ExternalBetId: model.ExternalBetId,
			ExternalCreatedOn: model.ExternalCreatedOn,
			ExternalClosedOn: model.ExternalClosedOn,
			ExternalPayoffOn: model.ExternalPayoffOn,
			ExternalResettleOn: model.ExternalResettleOn,
			BetState: model.BetState,
			BetType: model.BetType,
			FreeGameState: model.FreeGameState,
			GambleState: model.GambleState,
			CashoutState: model.CashoutState,
			ResettleState: model.ResettleState,
			JackpotState: model.JackpotState,
			CurrencyRatio: model.CurrencyRatio,
			Bet: model.Bet,
			ValidBet: model.ValidBet,
			Settle: model.Settle,
			Commission: model.Commission,
			Turnover: model.Turnover,
			WinLoss: model.WinLoss,
			Jackpot: model.Jackpot,
			CreatedOn: model.CreatedOn,
			ClosedOn: model.ClosedOn,
			UpdatedOn: model.UpdatedOn,
			ResetState: model.ResetState,
			ResetOn: model.ResetOn,
			AdditionalData: model.AdditionalData);
	}
}
