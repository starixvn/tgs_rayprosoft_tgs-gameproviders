using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.Joker.Client;
using Tgs.GameProviders.Core.Joker.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.Joker;

public class JokerGameUrlGetServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeBrand = Substitute.For<IBrandRepository>();
		var fakeGame = Substitute.For<IGameRepository>();
		var fakeGameProvider = Substitute.For<IGameProviderRepository>();
		var fakeClient = Substitute.For<IGameInterfaceJokerClient>();
		var fakePlayer = Substitute.For<IPlayerRepository>();

		var query = new JokerGameUrlQuery
		{
			PlayerCode = "4ucmqplxb029",
			GameId = 1,
			LoginRecordId = Guid.NewGuid().ToString(),
			Language = "zh-CN",
			PlatformType = PlatformType.Desktop,
			IPAddress = "127.0.0.1",
			BackUrl = new("https://dev-t1.rpgp.cc/")
		};

		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProvider
		.GetGameProviderIdAsync(
			gameProviderCode: Arg.Is(GameProviderType.Joker),
			cancellationToken: Arg.Any<CancellationToken>())
		.Returns(gameProviderId);

		var playerModel = new PlayerInfo(
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
		_ = fakePlayer
			.GetAsync(
				playerCode: Arg.Is(query.PlayerCode),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(playerModel);

		var gameProviderConfigurationId = Guid.NewGuid();
		_ = fakeBrand
			.GetBrandGameProviderConfigurationIdAsync(
				gameProviderId: Arg.Is(gameProviderId),
				brandId: Arg.Is(playerModel.BrandId),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameProviderConfigurationId);

		var configuration = new JokerConfiguration
		{
			ApiUrl = new("http://localhost/"),
			GameLaunchUrl = new Uri("http://localhost/"),
			AppId = "appId",
			SecretKey = "secretKey"
		};
		_ = fakeGameProvider
			.GetJokerConfigurationAsync(
				gameProviderConfigurationId: Arg.Is(gameProviderConfigurationId),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(configuration);

		var gameProviderCultureCode = "en";
		_ = fakeGameProvider
			.GetGameProviderCultureCodeAsync(
				gameProviderId: Arg.Is(gameProviderId),
				cultureCode: Arg.Is(query.Language),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameProviderCultureCode);

		var gameInfo = new GameInfo(
			GameId: 1,
			GameCode: "test",
			GameProviderId: gameProviderId,
			GameProviderCode: GameProviderType.Joker,
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
			SubGameProviderCode: GameProviderType.Joker);
		_ = fakeGame
			.GetAsync(
				Arg.Is(query.GameId),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameInfo);

		var gameToken = "game_token";
		_ = fakeClient
			 .GameTokenAsync(
				 apiUrl: Arg.Is(configuration.ApiUrl),
				 appId: Arg.Is(configuration.AppId),
				 secretKey: Arg.Is(configuration.SecretKey),
				 playerCode: Arg.Is(query.PlayerCode))
			 .Returns(gameToken);

		var sut = new JokerGameUrlGetService(
			fakeBrand,
			fakeGame,
			fakeGameProvider,
			fakeClient,
			fakePlayer);

		var actual = await sut.GetAsync(query);

		Assert.NotNull(actual);

		var queryStrings = HttpUtility.ParseQueryString(string.Empty);
		queryStrings.Add("token", gameToken);
		queryStrings.Add("game", gameInfo.GameExternalId);
		queryStrings.Add("mobile", (query.PlatformType == PlatformType.Mobile).ToString().ToLower());
		queryStrings.Add("lang", (!string.IsNullOrEmpty(gameProviderCultureCode)) ? "en" : gameProviderCultureCode);

		Assert.Equal($"{configuration.GameLaunchUrl.OriginalString}?{queryStrings}", actual);
	}
}
