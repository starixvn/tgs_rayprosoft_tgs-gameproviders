using System;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameProviders.Core.CQ9Gaming.ApplicationServices;
using System.Linq;
using Tgs.GameProviders.Domains.RoundDetail;

namespace Tgs.GameProviders.Core.UnitTest.CQ9Gaming;

public class CQ9GamingRoundDetailGetServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeGameBetRepository = Substitute.For<IGameBetRepository>();

		var query = new CQ9GamingRoundDetailQuery
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
			ExternalRoundId: "CQ9Gaming_mimi_mm-136817074",
			ExternalBetId: "CQ9Gaming_mimi_mm-136817074",
			ExternalCreatedOn: new DateTimeOffset(2024, 04, 18, 13, 14, 00, TimeSpan.FromHours(8)),
			ExternalClosedOn: new DateTimeOffset(2024, 04, 18, 13, 14, 00, TimeSpan.FromHours(8)),
			ExternalPayoffOn: new DateTimeOffset(2024, 04, 18, 13, 14, 00, TimeSpan.FromHours(8)),
			ExternalResettleOn: DateTimeOffset.UnixEpoch,
			BetState: GameBets.Common.Enums.BetState.Win,
			BetType: GameBets.Common.Enums.BetType.Normal,
			FreeGameState: GameBets.Common.Enums.FreeGameState.None,
			GambleState: GameBets.Common.Enums.GambleState.None,
			CashoutState: GameBets.Common.Enums.CashoutState.None,
			ResettleState: GameBets.Common.Enums.ResettleState.None,
			JackpotState: GameBets.Common.Enums.JackpotState.None,
			CurrencyRatio: 1.000000m,
			Bet: 3.000000m,
			ValidBet: 3.000000m,
			Settle: 6.000000m,
			Commission: 0.0m,
			Turnover: 3.000000m,
			WinLoss: 3.000000m,
			Jackpot: 0.0m,
			CreatedOn: DateTimeOffset.UtcNow,
			ClosedOn: DateTimeOffset.UtcNow,
			UpdatedOn: DateTimeOffset.UtcNow,
			ResetState: GameBets.Common.Enums.ResetState.None,
			ResetOn: DateTimeOffset.UnixEpoch,
			AdditionalData: new JsonObject
			{
				["refNo"] = "CQ9Gaming_mimi_mm-136817074",
				["status"] = "W",
				["betTime"] = "2024-04-18 05:14:10",
				["winloss"] = 3.0,
				["currency"] = "CNY",
				["gameCode"] = "1302642",
				["gameName"] = "Super All Stars",
				["playerId"] = "4ufhtneqo2ia",
				["betAmount"] = 3.0,
				["statementDate"] = "2024-04-18",
				["effectiveStake"] = 3.0,
				["isTransferGame"] = false
			}.ToJsonString());
		_ = fakeGameBetRepository.GetAsync(
			gameBetId: Arg.Is(query.GameBetId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameBetInfo);

		var sut = new CQ9GamingRoundDetailGetService(
			fakeGameBetRepository);

		var actual = await sut.GetAsync(query);

		Assert.NotNull(actual);
		Assert.NotEmpty((actual.Data!.First()! as RoundDetailDataJson)!.Value!);
	}
}
