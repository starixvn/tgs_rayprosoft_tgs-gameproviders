using System;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameProviders.Core.CRSports.ApplicationServices;
using Tgs.GameProviders.Domains.RoundDetail;

namespace Tgs.GameProviders.Core.UnitTest.CRSports;

public class CRSportsRoundDetailGetServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeGameBetRepository = Substitute.For<IGameBetRepository>();

		var query = new CRSportsRoundDetailQuery
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
			PlayerCode: "4uf6k014jp5x",
			CurrencyCode: "CNY",
			WalletId: Guid.NewGuid(),
			ExternalRoundId: "0351406A0199430B000100000006EA15035B",
			ExternalBetId: "438050329",
			ExternalCreatedOn: new DateTimeOffset(2022, 02, 16, 06, 35, 40, 497, TimeSpan.Zero),
			ExternalClosedOn: new DateTimeOffset(2022, 02, 16, 06, 34, 37, 113, TimeSpan.Zero),
			ExternalPayoffOn: new DateTimeOffset(2022, 02, 16, 06, 34, 37, 113, TimeSpan.Zero),
			ExternalResettleOn: DateTimeOffset.UnixEpoch,
			BetState: GameBets.Common.Enums.BetState.Loss,
			BetType: GameBets.Common.Enums.BetType.Normal,
			FreeGameState: GameBets.Common.Enums.FreeGameState.None,
			GambleState: GameBets.Common.Enums.GambleState.None,
			CashoutState: GameBets.Common.Enums.CashoutState.None,
			ResettleState: GameBets.Common.Enums.ResettleState.None,
			JackpotState: GameBets.Common.Enums.JackpotState.None,
			CurrencyRatio: 1.0m,
			Bet: 2.0m,
			ValidBet: 1.0m,
			Settle: 1.0m,
			Commission: 0.0m,
			Turnover: 0.0m,
			WinLoss: -1.0m,
			Jackpot: 0.0m,
			CreatedOn: DateTimeOffset.UtcNow,
			ClosedOn: DateTimeOffset.UtcNow,
			UpdatedOn: DateTimeOffset.UtcNow,
			ResetState: GameBets.Common.Enums.ResetState.None,
			ResetOn: DateTimeOffset.UnixEpoch,
			AdditionalData: JsonNode.Parse("""
			{
				"oddsFormat": "0.5 / 1",
				"strong": "C",
				"gtype": "FT",
				"degold": "2.000",
				"settle": "1",
				"mid": "28831508",
				"wingold_d": "0.95",
				"orderdate": "2024-07-26",
				"wingold": "1.00",
				"gold": "2.000",
				"result": "L",
				"rtype": "Running Ball-Home",
				"score": "0:0",
				"tname_home": "Real Murcia",
				"gold_d": "1.91",
				"cashoutid": "0",
				"odds": "H",
				"wtypecode": "RE",
				"currency": "HKD",
				"degold_d": "1.910",
				"id": "438050329",
				"adddate": "2024-07-26 04:31:51",
				"order": "Real Murcia",
				"cashout_d": "0.000",
				"tname_away": "Millwall",
				"handicap": "2",
				"pname": "",
				"IP": "185.196.0.16",
				"league": "Club Friendlies",
				"ioratio": "1.04",
				"rtypecode": "REH",
				"members_vgold": "1",
				"ordertime": "04:30:00",
				"cashout": "0.000",
				"report_test": "",
				"wtype": "Running Ball",
				"resultdetail": "LL",
				"vgold": "0.96",
				"resultdate": "2024-07-26 06:19:25",
				"result_score": "1:2",
				"username": "4uf6k014jp5x"
			}

			""")!.ToJsonString());
		_ = fakeGameBetRepository.GetAsync(
			gameBetId: Arg.Is(query.GameBetId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameBetInfo);

		var sut = new CRSportsRoundDetailGetService(
			fakeGameBetRepository);

		var actual = await sut.GetAsync(query);

		Assert.NotNull(actual);
		Assert.NotEmpty((actual.Data!.First()! as RoundDetailDataJson)!.Value!);
	}
}
