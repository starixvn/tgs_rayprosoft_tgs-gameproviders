using System;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameProviders.Core.GA28.ApplicationServices;

using Tgs.GameProviders.Domains.RoundDetail;

namespace Tgs.GameProviders.Core.UnitTest.GA28;

public class GA28RoundDetailGetServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeGameBetRepository = Substitute.For<IGameBetRepository>();

		var query = new GA28RoundDetailQuery
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
			ExternalRoundId: "ph#74 M000478",
			ExternalBetId: "190103969DD96J562D",
			ExternalCreatedOn: new DateTimeOffset(2024, 06, 13, 14, 11, 56, 510, 124, TimeSpan.FromHours(8)),
			ExternalClosedOn: new DateTimeOffset(2024, 06, 13, 14, 12, 29, 238, 152, TimeSpan.FromHours(8)),
			ExternalPayoffOn: new DateTimeOffset(2024, 06, 13, 14, 12, 29, 238, 152, TimeSpan.FromHours(8)),
			ExternalResettleOn: DateTimeOffset.UnixEpoch,
			BetState: GameBets.Common.Enums.BetState.Win,
			BetType: GameBets.Common.Enums.BetType.Normal,
			FreeGameState: GameBets.Common.Enums.FreeGameState.None,
			GambleState: GameBets.Common.Enums.GambleState.None,
			CashoutState: GameBets.Common.Enums.CashoutState.None,
			ResettleState: GameBets.Common.Enums.ResettleState.None,
			JackpotState: GameBets.Common.Enums.JackpotState.None,
			CurrencyRatio: 1m,
			Bet: 10.000000m,
			ValidBet: 10.000000m,
			Settle: 19.300000m,
			Commission: 0.0m,
			Turnover: 10.000000m,
			WinLoss: 9.300000m,
			Jackpot: 0.0m,
			CreatedOn: DateTimeOffset.UtcNow,
			ClosedOn: DateTimeOffset.UtcNow,
			UpdatedOn: DateTimeOffset.UtcNow,
			ResetState: GameBets.Common.Enums.ResetState.None,
			ResetOn: DateTimeOffset.UnixEpoch,
			AdditionalData: new JsonObject
			{
				["loginName"] = "4ucmqplxb029",
				["orderId"] = "QXT24060517201133513",
				["state"] = "2",
				["createDate"] = "2024-06-05 17:20:11",
				["resultDate"] = "2024-06-06 04:18:58",
				["realResultDate"] = "2024-06-06 04:18:58",
				["gold"] = 10.000000m,
				["playerId"] = 10.000000m,
				["realGold"] = 3.0,
				["winGold"] = 9.300000m,
				["cashoutType"] = 0
			}.ToJsonString());
		_ = fakeGameBetRepository.GetAsync(
			gameBetId: Arg.Is(query.GameBetId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameBetInfo);

		var sut = new GA28RoundDetailGetService(
			fakeGameBetRepository);

		var actual = await sut.GetAsync(query);

		Assert.NotNull(actual);
		Assert.NotEmpty((actual.Data!.First()! as RoundDetailDataJson)!.Value!);
	}
}
