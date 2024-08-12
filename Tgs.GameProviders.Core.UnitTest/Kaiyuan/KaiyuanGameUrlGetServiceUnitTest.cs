using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.Kaiyuan.Client;
using Tgs.GameProviders.Core.Kaiyuan.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.Kaiyuan;

public class KaiyuanGameUrlGetServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeClient = Substitute.For<IGameInterfaceKaiyuanClient>();
		var fakeGameProvider = Substitute.For<IGameProviderRepository>();
		var fakePlayer = Substitute.For<IPlayerRepository>();
		var fakeBrand = Substitute.For<IBrandRepository>();
		var fakeGame = Substitute.For<IGameRepository>();

		var query = new KaiyuanGameUrlQuery
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
		_ = fakeGameProvider.GetGameProviderIdAsync(
			gameProviderCode: Arg.Is(GameProviderType.Kaiyuan),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(ValueTask.FromResult(gameProviderId));

		var player = new PlayerInfo(
			ResellerId: Guid.NewGuid(),
			ResellerCode: "GPI",
			LicenseeId: Guid.NewGuid(),
			LicenseeCode: "GPI",
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			PlayerCode: "test",
			Username: "test",
			DefaultWalletId: Guid.NewGuid(),
			DefaultWalletCurrencyCode: "HKD");
		_ = fakePlayer
			.GetAsync(
			Arg.Is(query.PlayerCode),
			Arg.Any<CancellationToken>())
			.Returns(player);

		var gameProviderConfigurationId = Guid.NewGuid();
		_ = fakeBrand
			.GetBrandGameProviderConfigurationIdAsync(
			gameProviderId: Arg.Is(gameProviderId),
			brandId: Arg.Is(player.BrandId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(ValueTask.FromResult(gameProviderConfigurationId));

		var configuration = new KaiyuanConfiguration
		{
			AmountMaximum = 999999999999,
			AmountFormat = "12,2",
			ApiUrl = new("http://localhost/"),
			ApiRecordUrl = new("http://localhost:8090/"),
			Deskey = "deskey",
			Md5key = "md5key",
			CurrencyCode = "HKD",
			AgentId = "200995"
		};
		_ = fakeGameProvider
			.GetKaiyuanConfigurationAsync(
			gameProviderConfigurationId: Arg.Is(gameProviderConfigurationId),
			cancellationToken: Arg.Any<CancellationToken>()).
			Returns(ValueTask.FromResult(configuration));

		var gameInfo = new GameInfo(
			GameId: 1,
			GameCode: "test",
			GameProviderId: gameProviderId,
			GameProviderCode: GameProviderType.Kaiyuan,
			GameExternalId: "0",
			Type: GameType.CardGameLobby,
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
			SubGameProviderCode: GameProviderType.Kaiyuan);
		_ = fakeGame.GetAsync(
			gameId: Arg.Is(query.GameId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameInfo);

		var gameurl = "https://localhost";
		_ = fakeClient
			.GameUrlAsync(
			apiUrl: Arg.Is(configuration.ApiUrl),
			deskey: Arg.Is(configuration.Deskey),
			md5key: Arg.Is(configuration.Md5key),
			agentId: Arg.Is(configuration.AgentId),
			brandCode: Arg.Is(player.BrandCode),
			playerCode: Arg.Is(player.PlayerCode),
			ipAddress: Arg.Is(query.IPAddress),
			externalGameId: Arg.Is(gameInfo.GameExternalId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameurl);

		var sut = new KaiyuanGameUrlGetService(
			fakeGameProvider,
			fakePlayer,
			fakeBrand,
			fakeGame,
			fakeClient);

		var actual = await sut.GetAsync(query);

		Assert.NotNull(actual);
		Assert.Equal(gameurl, actual);
	}
}
