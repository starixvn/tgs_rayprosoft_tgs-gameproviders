using System;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameProviders.Core.GTExpanse.ApplicationServices;
using Tgs.GameProviders.Domains.RoundDetail;

namespace Tgs.GameProviders.Core.UnitTest.GTExpanse;

public class GTExpanseRoundDetailGetServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeGameBetRepository = Substitute.For<IGameBetRepository>();

		var query = new GTExpanseRoundDetailQuery
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
			ExternalRoundId: "gpas_3_CT76ykoNwUhtp7E5X3",
			ExternalBetId: "4dae8cebcfaf4e0191e437e1c295deca",
			ExternalCreatedOn: DateTimeOffset.Parse("2024-07-08T07:06:01+00:00"),
			ExternalClosedOn: DateTimeOffset.Parse("2024-07-08T07:06:02.608+00:00"),
			ExternalPayoffOn: DateTimeOffset.Parse("2024-07-08T07:06:02.608+00:00"),
			ExternalResettleOn: DateTimeOffset.UnixEpoch,
			BetState: GameBets.Common.Enums.BetState.Loss,
			BetType: GameBets.Common.Enums.BetType.Normal,
			FreeGameState: GameBets.Common.Enums.FreeGameState.None,
			GambleState: GameBets.Common.Enums.GambleState.None,
			CashoutState: GameBets.Common.Enums.CashoutState.None,
			ResettleState: GameBets.Common.Enums.ResettleState.None,
			JackpotState: GameBets.Common.Enums.JackpotState.None,
			CurrencyRatio: 1.000000m,
			Bet: 0.250000m,
			ValidBet: 0.250000m,
			Settle: 0.000000m,
			Commission: 0.0m,
			Turnover: 0.250000m,
			WinLoss: -0.250000m,
			Jackpot: 0.0m,
			CreatedOn: DateTimeOffset.UtcNow,
			ClosedOn: DateTimeOffset.UtcNow,
			UpdatedOn: DateTimeOffset.UtcNow,
			ResetState: GameBets.Common.Enums.ResetState.None,
			ResetOn: DateTimeOffset.UnixEpoch,
			AdditionalData: JsonNode.Parse(""""
				{
					"roundId": "gpas_3_CT76ykoNwUhtp7E5X3",
					"playerId": "4ucmqplxb029",
					"brandId": 908,
					"providerCode": "pt",
					"sessionId": "4dae8cebcfaf4e0191e437e1c295deca",
					"roundType": "normal",
					"roundStatus": "end",
					"gameCode": "gpas_dostormspp_pop",
					"currency": "CNY",
					"platform": "mobile",
					"bets": 0.25,
					"wins": 0,
					"cancels": 0,
					"transIn": 0,
					"transOut": 0,
					"balanceEnd": 99.75,
					"bonusBalanceEnd": 0,
					"rake": 0,
					"startTime": "2024-07-08 07:06:01.000",
					"endTime": "2024-07-08 07:06:02.608",
					"jpc": 0.0075,
					"jpw": 0,
					"jpDetails": {
						"type": "json",
						"value": "{\\u0022type\\u0022:\\u0022json\\u0022,\\u0022value\\u0022:\\u0022[{\\\\\\u0022id\\\\\\u0022:\\\\\\u0022ptjp_114230_114240_114250_306\\\\\\u0022,\\\\\\u0022contribution\\\\\\u0022:0.007475}]\\u0022,\\u0022null\\u0022:true}",
						"null": true
					},
					"detailUrl": "https://extstg3-cashier01.ptstaging.eu/getgamehistoryurl.php?casino=ptstaging3.33\\u0026username=1CLUB__PEG_4ucmqplxb029\\u0026ngscode=21467861\\u0026remotesessionenddate=2024-07-08\\u002B07%3A06%3A01\\u0026token=38bd8cb9ccad811308e96a8b35f1eb5908a62e44da2d269c0d392d89a22e493f\\u0026language=EN\\u0026showFullHistoryPerBet=false",
					"gameType": "slots"
				}
				"""")!);
		_ = fakeGameBetRepository.GetAsync(
			gameBetId: Arg.Is(query.GameBetId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameBetInfo);

		var sut = new GTExpanseRoundDetailGetService(
			fakeGameBetRepository);

		var actual = await sut.GetAsync(query);

		Assert.NotNull(actual);
		Assert.NotEmpty((actual.Data!.First()! as RoundDetailDataUrl)!.Url);
	}
}
