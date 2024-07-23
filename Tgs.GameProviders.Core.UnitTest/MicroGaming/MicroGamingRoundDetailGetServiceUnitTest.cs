using System;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameProviders.Core.MicroGaming.ApplicationServices;
using Tgs.GameProviders.Domains.RoundDetail;

namespace Tgs.GameProviders.Core.UnitTest.MicroGaming;

public class MicroGamingRoundDetailGetServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeGameBetRepository = Substitute.For<IGameBetRepository>();

		var query = new MicroGamingRoundDetailQuery
		{
			GameBetId = Guid.NewGuid(),
			Language = "en"
		};

		var gameBetInfo = new GameBetInfo(
			Id: query.GameBetId,
			GameProviderId: Guid.NewGuid(),
			GameProviderConfigurationId: Guid.NewGuid(),
			GameId: 1,
			ResellerId: Guid.NewGuid(),
			LicenseeId: Guid.NewGuid(),
			BrandId: Guid.NewGuid(),
			PlayerCode: "4ucmqplxb029",
			CurrencyCode: "CNY",
			WalletId: Guid.NewGuid(),
			ExternalRoundId: "0351406A0199430B000100000006EA15035B",
			ExternalBetId: "0351406A0199430B000100000006EA15035B",
			ExternalCreatedOn: new DateTimeOffset(2022, 02, 16, 06, 35, 40, 497, TimeSpan.Zero),
			ExternalClosedOn: new DateTimeOffset(2022, 02, 16, 06, 34, 37, 113, TimeSpan.Zero),
			ExternalPayoffOn: new DateTimeOffset(2022, 02, 16, 06, 34, 37, 113, TimeSpan.Zero),
			ExternalResettleOn: DateTimeOffset.UnixEpoch,
			BetState: GameBets.Common.Enums.BetState.Win,
			BetType: GameBets.Common.Enums.BetType.Normal,
			FreeGameState: GameBets.Common.Enums.FreeGameState.None,
			GambleState: GameBets.Common.Enums.GambleState.None,
			CashoutState: GameBets.Common.Enums.CashoutState.None,
			ResettleState: GameBets.Common.Enums.ResettleState.None,
			JackpotState: GameBets.Common.Enums.JackpotState.None,
			CurrencyRatio: 1.0m,
			Bet: 3.750m,
			ValidBet: 3.750m,
			Settle: 7.50m,
			Commission: 0.0m,
			Turnover: 0.0m,
			WinLoss: -3.750m,
			Jackpot: 0.0m,
			CreatedOn: DateTimeOffset.UtcNow,
			ClosedOn: DateTimeOffset.UtcNow,
			UpdatedOn: DateTimeOffset.UtcNow,
			ResetState: GameBets.Common.Enums.ResetState.None,
			ResetOn: DateTimeOffset.UnixEpoch,
			AdditionalData: JsonNode.Parse("""
			{
				"betUID": "0351406A0199430B000100000006EA15035B",
				"createdDateUTC": "2022-02-16T06:35:40.497",
				"gameStartTimeUTC": "2022-02-16T06:34:37.113",
				"gameEndTimeUTC": "2022-02-16T06:34:37.113",
				"playerId": "4ucmqplxb029",
				"productId": "SMG",
				"productPlayerId": "M0G295682018",
				"platform": "Desktop",
				"gameCode": "SMG_108Heroes",
				"currency": "CNY",
				"betAmount": 3.7500,
				"payoutAmount": 0.0000,
				"betStatus": "Closed",
				"PCA": null,
				"externalTransactionId": "1005639168/720",
				"metadata": {
					"SessionId": "1005639168",
					"TransactionId": "720"
				}
			}
			""")!.ToJsonString());
		_ = fakeGameBetRepository.GetAsync(
			gameBetId: Arg.Is(query.GameBetId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameBetInfo);

		var sut = new MicroGamingRoundDetailGetService(
			fakeGameBetRepository);

		var actual = await sut.GetAsync(query);

		Assert.NotNull(actual);
		Assert.NotEmpty((actual.Data!.First()! as RoundDetailDataJson)!.Value!);
	}
}
