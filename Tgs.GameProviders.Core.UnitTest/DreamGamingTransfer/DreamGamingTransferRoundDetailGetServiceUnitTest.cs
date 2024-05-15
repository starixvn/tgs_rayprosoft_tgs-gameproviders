using System;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameInterface.DreamGamingService.Client;
using Tgs.GameInterface.DreamGamingService.Protos;
using Tgs.GameProviders.Core.DreamGamingTransfer.ApplicationServices;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.DreamGamingTransfer;

public class DreamGamingTransferRoundDetailGetServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeGameBetRepository = Substitute.For<IGameBetRepository>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakeClient = Substitute.For<IGameInterfaceDreamGamingClient>();

		var query = new DreamGamingTransferRoundDetailQuery
		{
			GameBetId = Guid.NewGuid(),
			Language = "en-us"
		};

		var gameBetInfo = new GameBetInfo(
			Id: query.GameBetId,
			GameProviderId: Guid.NewGuid(),
			GameProviderConfigurationId: Guid.NewGuid(),
			GameId: 5000007,
			ResellerId: Guid.NewGuid(),
			LicenseeId: Guid.NewGuid(),
			BrandId: Guid.NewGuid(),
			PlayerCode: "4ucmqplxb029",
			CurrencyCode: "CNY",
			WalletId: Guid.NewGuid(),
			ExternalRoundId: "20417330944",
			ExternalBetId: "20417330944",
			ExternalCreatedOn: new DateTimeOffset(2024, 05, 15, 5, 40, 36, TimeSpan.FromHours(8)),
			ExternalClosedOn: new DateTimeOffset(2024, 05, 15, 5, 40, 55, TimeSpan.FromHours(8)),
			ExternalPayoffOn: new DateTimeOffset(2024, 05, 15, 5, 40, 55, TimeSpan.FromHours(8)),
			ExternalResettleOn: DateTimeOffset.UnixEpoch,
			BetState: GameBets.Common.Enums.BetState.Loss,
			BetType: GameBets.Common.Enums.BetType.Normal,
			FreeGameState: GameBets.Common.Enums.FreeGameState.None,
			GambleState: GameBets.Common.Enums.GambleState.None,
			CashoutState: GameBets.Common.Enums.CashoutState.None,
			ResettleState: GameBets.Common.Enums.ResettleState.None,
			JackpotState: GameBets.Common.Enums.JackpotState.None,
			CurrencyRatio: 1.000000m,
			Bet: 99.000000m,
			ValidBet: 99.000000m,
			Settle: 0.000000m,
			Commission: 0.000000m,
			Turnover: 99.000000m,
			WinLoss: -99.000000m,
			Jackpot: 0.000000m,
			CreatedOn: new DateTimeOffset(2024, 05, 15, 5, 40, 36, TimeSpan.FromHours(8)),
			ClosedOn: new DateTimeOffset(2024, 05, 15, 5, 41, 16, TimeSpan.FromHours(8)),
			UpdatedOn: new DateTimeOffset(2024, 05, 15, 5, 41, 16, TimeSpan.FromHours(8)),
			ResetState: GameBets.Common.Enums.ResetState.None,
			ResetOn: DateTimeOffset.UnixEpoch,
			AdditionalData: JsonNode.Parse("{\"id\": 20417330944, \"ip\": \"112.198.205.130\", \"ext\": \"240515B010432\", \"gameId\": 1, \"playId\": 4, \"result\": \"{\\\"result\\\":\\\"7,2,6\\\",\\\"poker\\\":{\\\"banker\\\":\\\"18-39-37\\\",\\\"player\\\":\\\"21-8-0\\\"}}\", \"roadid\": 0, \"shoeId\": 9, \"betTime\": \"2024-05-15 05:40:36\", \"calTime\": \"2024-05-15 05:40:55\", \"lobbyId\": 1, \"tableId\": 10101, \"gameType\": 1, \"memberId\": 96421283, \"parentId\": 23957, \"pluginid\": 0, \"userName\": \"4UHCFHNWW2O9\", \"betDetail\": \"{\\\"banker\\\":99.0}\", \"betPoints\": 99.0, \"winOrLoss\": 0.0, \"betPointsz\": 0.0, \"currencyId\": 7, \"deviceType\": 5, \"availableBet\": 99.0, \"isRevocation\": 1, \"balanceBefore\": 99.14999999999998}")!);

		_ = fakeGameBetRepository.GetAsync(
			gameBetId: Arg.Is(query.GameBetId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameBetInfo);

		var configuration = new DreamGamingTransferConfiguration()
		{
			VideoGameApiUrl = new Uri("https://ag.dg66.info/"),
			ApiUrl = new("https://api.dg0.co/"),
			ApiKey = "d39da9da731b427b9349b80d9d97f2e4",
			AgentName = "DGTE0105IH",
		};
		_ = fakeGameProviderRepository.GetDreamGamingTransferConfigurationAsync(
			Arg.Is(gameBetInfo.GameProviderConfigurationId)).Returns(configuration);

		var gameProviderCultureCode = "en";
		_ = fakeGameProviderRepository.GetGameProviderCultureCodeAsync(
			Arg.Is(gameBetInfo.GameProviderId),
			Arg.Is(query.Language)).Returns(gameProviderCultureCode);

		var response = new GameHistoryDetailReply()
		{
			GameResult = "https://ag.dg66.info/ag/result/result.html?language=en&agentFix=3213&id=13540803433"
		};

		_ = fakeClient
			.GameHistoryDetailAsync(
				Arg.Is(configuration.VideoGameApiUrl),
				Arg.Is(configuration.AgentName),
				gameProviderCultureCode,
				gameBetInfo.ExternalRoundId,
				Arg.Any<CancellationToken>())
			.Returns(response);

		var sut = new DreamGamingTransferRoundDetailGetService(
			fakeGameBetRepository,
			fakeGameProviderRepository,
			fakeClient);

		var actual = await sut.GetAsync(query);

		Assert.NotNull(actual);

		_ = fakeClient
			.Received(1)
			.GameHistoryDetailAsync(
			Arg.Is(configuration.VideoGameApiUrl),
			Arg.Is(configuration.AgentName),
			Arg.Is(gameProviderCultureCode),
			Arg.Is(gameBetInfo.ExternalRoundId));
	}
}
