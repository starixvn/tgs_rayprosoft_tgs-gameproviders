using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.Funky.Protos;
using Tgs.GameInterface.Funky.Client;
using Tgs.GameProviders.Core.Funky.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;
using System.Text.Json.Nodes;
using System.Linq;
using Tgs.GameProviders.Domains.RoundDetail;

namespace Tgs.GameProviders.Core.UnitTest.Funky;
public class FunkyRoundDetailGetServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeRoundRepository = Substitute.For<IRoundRepository>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeBrandRepository = Substitute.For<IBrandRepository>();
		var fakeGameRepository = Substitute.For<IGameRepository>();
		var fakeClient = Substitute.For<IGameInterfaceFunkyClient>();

		var query = new FunkyRoundDetailQuery
		{
			RoundId = Guid.NewGuid(),
			Language = "en"
		};

		var roundInfo = new RoundInfo(
			RoundId: query.RoundId,
			ExternalRoundId: "Funky_tt_4xGoldenDynastyx5199x82477",
			GameProviderId: Guid.NewGuid(),
			PlayerCode: "4ucmqplxb029",
			GameId: 1,
			CurrencyCode: "CNY",
			Status: GameRounds.Common.GameRoundStatus.Closed,
			GameProviderConfigurationId: Guid.NewGuid(),
			WalletId: Guid.NewGuid(),
			CreatedOn: DateTimeOffset.UtcNow,
			ClosedOn: DateTimeOffset.UtcNow,
			UpdatedOn: DateTimeOffset.UtcNow,
			BrandId: Guid.NewGuid(),
			ExternalCreatedOn: new DateTimeOffset(2024, 03, 19, 03, 14, 02, TimeSpan.Zero),
			ExternalClosedOn: new DateTimeOffset(2024, 03, 19, 03, 14, 02, TimeSpan.Zero),
			BetAmount: 0.76m,
			ValidBetAmount: 0.76m,
			SettleAmount: 6.0m,
			Commission: 0.0m,
			Turnover: 0.76m,
			CompanyWinLoss: -5.24m,
			TieBetAdjustment: 0.0m,
			ClientStatus: GameRounds.Common.GameRoundClientStatus.Win,
			ReopenTurnoverAdjustment: null,
			ReopenedOn: null,
			EffectiveBettingPercentage: null,
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
		_ = fakeRoundRepository.GetAsync(
			 roundId: Arg.Is(query.RoundId),
			 cancellationToken: Arg.Any<CancellationToken>())
			.Returns(roundInfo);

		_ = fakeGameProviderRepository.GetGameProviderIdAsync(
			gameProviderCode: Arg.Is(GameProviderType.Funky),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(roundInfo.GameProviderId);

		var player = new PlayerInfo(
			ResellerId: Guid.NewGuid(),
			ResellerCode: "GPI",
			LicenseeId: Guid.NewGuid(),
			LicenseeCode: "GPI",
			BrandId: roundInfo.BrandId,
			BrandCode: "GPI",
			PlayerCode: "4ucmqplxb029",
			Username: "4ucmqplxb029",
			DefaultWalletId: Guid.NewGuid(),
			DefaultWalletCurrencyCode: "CNY");
		_ = fakePlayerRepository.GetAsync(
			playerCode: Arg.Is(roundInfo.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(player);

		_ = fakeBrandRepository.GetBrandGameProviderConfigurationIdAsync(
			gameProviderId: Arg.Is(roundInfo.GameProviderId),
			brandId: Arg.Is(player.BrandId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(roundInfo.GameProviderConfigurationId);

		var configuration = new FunkyConfiguration()
		{
			ApiUrl = new("http://trial-gp-api.funkytest.com/"),
			ApiReportUrl = new("http://trial-gp-api-report.funkytest.com/"),
			Authentication = "c69d57e5-93e7-488c-90a4-e20876b495bc",
			UserAgent = "109-PG"
		};
		_ = fakeGameProviderRepository.GetFunkyConfigurationAsync(
			gameProviderConfigurationId: Arg.Is(roundInfo.GameProviderConfigurationId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(configuration);

		var cultureCode = "EN";
		_ = fakeGameProviderRepository.GetGameProviderCultureCodeAsync(
			gameProviderId: Arg.Is(roundInfo.GameProviderId),
			cultureCode: Arg.Is(query.Language),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(cultureCode);

		var game = new GameInfo(
			GameId: roundInfo.GameId,
			GameCode: "test",
			GameProviderId: roundInfo.GameProviderId,
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
			gameId: Arg.Is(roundInfo.GameId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(game);

		var reply = "https://funky-test.ttstations.com/orderid/";
		_ = fakeClient.GameWagerDetailAsync(
			apiUrl: Arg.Is(configuration.ApiUrl),
			userAgent: Arg.Is(configuration.UserAgent),
			authentication: Arg.Is(configuration.Authentication),
			playerCode: Arg.Is(player.PlayerCode),
			gameCode: Arg.Is(game.GameExternalId),
			externalWagerId: Arg.Is(roundInfo.ExternalRoundId),
			language: Arg.Is(cultureCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(reply);

		var sut = new FunkyRoundDetailGetService(
			fakeRoundRepository,
			fakeGameProviderRepository,
			fakePlayerRepository,
			fakeBrandRepository,
			fakeGameRepository,
			fakeClient);

		var actual = await sut.GetAsync(query);

		Assert.NotNull(actual);
		Assert.Equal(reply, (actual.Data!.First()! as RoundDetailDataUrl)!.Url);
	}
}
