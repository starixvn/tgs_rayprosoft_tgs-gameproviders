using System;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.Funky.Client;
using Tgs.GameProviders.Core.Funky.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.RoundDetail;

using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.Funky;

public class FunkyRoundDetailGetServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeGameBetRepository = Substitute.For<IGameBetRepository>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakeGameRepository = Substitute.For<IGameRepository>();
		var fakeClient = Substitute.For<IGameInterfaceFunkyClient>();

		var query = new FunkyRoundDetailQuery
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
			ExternalRoundId: "Funky_tt_4xGoldenDynastyx5199x82477",
			ExternalBetId: "Funky_tt_4xGoldenDynastyx5199x82477",
			ExternalCreatedOn: new DateTimeOffset(2024, 03, 19, 03, 14, 02, TimeSpan.Zero),
			ExternalClosedOn: new DateTimeOffset(2024, 03, 19, 03, 14, 02, TimeSpan.Zero),
			ExternalPayoffOn: new DateTimeOffset(2024, 03, 19, 03, 14, 02, TimeSpan.Zero),
			ExternalResettleOn: DateTimeOffset.UnixEpoch,
			BetState: GameBets.Common.Enums.BetState.Win,
			BetType: GameBets.Common.Enums.BetType.Normal,
			FreeGameState: GameBets.Common.Enums.FreeGameState.None,
			GambleState: GameBets.Common.Enums.GambleState.None,
			CashoutState: GameBets.Common.Enums.CashoutState.None,
			ResettleState: GameBets.Common.Enums.ResettleState.None,
			JackpotState: GameBets.Common.Enums.JackpotState.None,
			CurrencyRatio: 1.000000m,
			Bet: 0.76m,
			ValidBet: 0.76m,
			Settle: 6.0m,
			Commission: 0.0m,
			Turnover: 0.76m,
			WinLoss: -5.24m,
			Jackpot: 0.000000m,
			CreatedOn: new DateTimeOffset(2024, 03, 19, 03, 14, 02, TimeSpan.Zero),
			ClosedOn: new DateTimeOffset(2024, 03, 19, 03, 14, 02, TimeSpan.Zero),
			UpdatedOn: new DateTimeOffset(2024, 03, 19, 03, 14, 02, TimeSpan.Zero),
			ResetState: GameBets.Common.Enums.ResetState.None,
			ResetOn: DateTimeOffset.UnixEpoch,
			AdditionalData: new JsonObject
			{
				["playerId"] = "4ucmqplxb029",
				["statementDate"] = "2024-03-19",
				["betTime"] = "2024-03-19 03:14:02",
				["refNo"] = "Funky_tt_4xGoldenDynastyx5199x82477",
				["status"] = "W",
				["gameCode"] = "10261",
				["gameName"] = "Golden Dynasty",
				["currency"] = "CNY",
				["betAmount"] = 0.76,
				["effectiveStake"] = 0.76,
				["winloss"] = 5.24,
				["isTransferGame"] = false
			}.ToJsonString());
		_ = fakeGameBetRepository.GetAsync(
			 gameBetId: Arg.Is(query.GameBetId),
			 cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameBetInfo);

		var configuration = new FunkyConfiguration()
		{
			ApiUrl = new("http://trial-gp-api.funkytest.com/"),
			ApiReportUrl = new("http://trial-gp-api-report.funkytest.com/"),
			Authentication = "c69d57e5-93e7-488c-90a4-e20876b495bc",
			UserAgent = "109-PG"
		};
		_ = fakeGameProviderRepository.GetFunkyConfigurationAsync(
			gameProviderConfigurationId: Arg.Is(gameBetInfo.GameProviderConfigurationId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(configuration);

		var cultureCode = "EN";
		_ = fakeGameProviderRepository.GetGameProviderCultureCodeAsync(
			gameProviderId: Arg.Is(gameBetInfo.GameProviderId),
			cultureCode: Arg.Is(query.Language),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(cultureCode);

		var game = new GameInfo(
			GameId: gameBetInfo.GameId,
			GameCode: "test",
			GameProviderId: gameBetInfo.GameProviderId,
			GameProviderCode: GameProviderType.Funky,
			GameExternalId: "1",
			Type: GameType.Slot,
			PlatformType: GamePlatformType.DesktopAndMobile,
			Name: "test",
			SubGame: true,
			AvailableToBrand: false,
			HasTurnoverOnTieBet: true,
			Html5: true,
			Description: null,
			EndpointPath: null,
			RuleUrl: null,
			AvailableCurrencies: null,
			Localization: null,
			SubGameProviderCode: GameProviderType.Funky);
		_ = fakeGameRepository.GetAsync(
			gameId: Arg.Is(gameBetInfo.GameId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(game);

		var reply = "https://funky-test.ttstations.com/orderid/";
		_ = fakeClient.GameWagerDetailAsync(
			apiUrl: Arg.Is(configuration.ApiUrl),
			userAgent: Arg.Is(configuration.UserAgent),
			authentication: Arg.Is(configuration.Authentication),
			playerCode: Arg.Is(gameBetInfo.PlayerCode),
			gameCode: Arg.Is(game.GameExternalId),
			externalWagerId: Arg.Is(gameBetInfo.ExternalBetId),
			language: Arg.Is(cultureCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(reply);

		var sut = new FunkyRoundDetailGetService(
			fakeGameBetRepository,
			fakeGameProviderRepository,
			fakeGameRepository,
			fakeClient);

		var actual = await sut.GetAsync(query);

		Assert.NotNull(actual);
		Assert.Equal(reply, (actual.Data!.First()! as RoundDetailDataUrl)!.Url);
	}
}
