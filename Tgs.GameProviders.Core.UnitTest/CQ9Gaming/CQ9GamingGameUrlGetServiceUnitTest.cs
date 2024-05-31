using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.CQ9Gaming.Client;
using Tgs.GameInterface.CQ9Gaming.Protos;
using Tgs.GameProviders.Core.CQ9Gaming.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.CQ9Gaming;

public class CQ9GamingGameUrlGetServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeBrandRepository = Substitute.For<IBrandRepository>();
		var fakeGameRepository = Substitute.For<IGameRepository>();
		var fakeClient = Substitute.For<IGameInterfaceCQ9GamingClient>();

		var query = new CQ9GamingGameUrlQuery
		{
			GameType = GameType.Slot,
			GameId = 1,
			BrandCode = "GPI",
			PlayerCode = "4ucmqplxb029",
			CurrencyCode = "CNY",
			LoginRecordId = Guid.NewGuid(),
			PlatformType = PlatformType.Desktop,
			Language = "zh-CN",
			IPAddress = "127.0.0.1",
			BackUrl = new("https://dev-t1.rpgp.cc/")
		};

		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProviderRepository.GetGameProviderIdAsync(
			gameProviderCode: Arg.Is(GameProviderType.CQ9Gaming),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameProviderId);

		var player = new PlayerInfo(
			ResellerId: Guid.NewGuid(),
			ResellerCode: "GPI",
			LicenseeId: Guid.NewGuid(),
			LicenseeCode: "GPI",
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			PlayerCode: "4ucmqplxb029",
			Username: "4ucmqplxb029",
			DefaultWalletId: Guid.NewGuid(),
			DefaultWalletCurrencyCode: "CNY");
		_ = fakePlayerRepository.GetAsync(
			playerCode: Arg.Is(query.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(player);

		var gameProviderConfigurationId = Guid.NewGuid();
		_ = fakeBrandRepository.GetBrandGameProviderConfigurationIdAsync(
			gameProviderId: Arg.Is(gameProviderId),
			brandId: Arg.Is(player.BrandId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameProviderConfigurationId);

		var configuration = new CQ9GamingConfiguration()
		{
			ApiUrl = new("https://localhost/"),
			ApiToken = "apiToken",
		};
		_ = fakeGameProviderRepository.GetCQ9GamingConfigurationAsync(
			gameProviderConfigurationId: Arg.Is(gameProviderConfigurationId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(configuration);

		var cultureCode = "en";
		_ = fakeGameProviderRepository.GetGameProviderCultureCodeAsync(
			gameProviderId: Arg.Is(gameProviderId),
			cultureCode: Arg.Is(query.Language),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(cultureCode);

		var game = new GameInfo(
			GameId: 1,
			GameCode: "test",
			GameProviderId: Guid.NewGuid(),
			GameProviderCode: GameProviderType.CQ9Gaming,
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
			SubGameProviderCode: GameProviderType.CQ9Gaming);
		_ = fakeGameRepository.GetAsync(
			gameId: Arg.Is(query.GameId.Value),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(game);

		var gameLoginReply = new GameLoginReply
		{
			UserToken = "userToken"
		};

		_ = fakeClient.GameLoginAsync(
			apiUrl: Arg.Is(configuration.ApiUrl),
			apiToken: Arg.Is(configuration.ApiToken),
			playerCode: Arg.Is(player.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameLoginReply);

		var gameUrlReply = new GameUrlReply
		{
			GameUrl = "https://localhost/"
		};

		_ = fakeClient.GameUrlAsync(
			apiUrl: Arg.Is(configuration.ApiUrl),
			apiToken: Arg.Is(configuration.ApiToken),
			userToken: Arg.Is(gameLoginReply.UserToken),
			gameCode: Arg.Is(game.GameExternalId),
			isMobile: Arg.Any<bool>(),
			language: Arg.Is(cultureCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameUrlReply);

		var sut = new CQ9GamingGameUrlGetService(
			fakeGameProviderRepository,
			fakePlayerRepository,
			fakeBrandRepository,
			fakeGameRepository,
			fakeClient);

		var actual = await sut.GetAsync(query);

		Assert.NotNull(actual);
		Assert.NotEmpty(actual.ToString());
	}
}
