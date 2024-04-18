using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.VRGaming.Client;
using Tgs.GameProviders.Core.VRGaming.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.VRGaming;

public class VRGamingGameUrlGetServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeClient = Substitute.For<IGameInterfaceVRGamingClient>();
		var fakeGameProvider = Substitute.For<IGameProviderRepository>();
		var fakePlayer = Substitute.For<IPlayerRepository>();
		var fakeBrand = Substitute.For<IBrandRepository>();
		var fakeGame = Substitute.For<IGameRepository>();

		var query = new VRGamingGameUrlQuery
		{
			GameType = GameType.CardGameLobby,
			GameId = 1,
			BrandCode = "GPI",
			PlayerCode = "test",
			CurrencyCode = "CNY",
			LoginRecordId = Guid.NewGuid(),
			PlatformType = PlatformType.Desktop,
			Language = "en-US",
			IPAddress = "127.0.0.1",
			BackUrl = new("http://localhost/")
		};

		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProvider.GetGameProviderIdAsync(
			gameProviderCode: Arg.Is(GameProviderType.VRGaming),
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

		var configuration = new VRGamingConfiguration
		{
			AmountMaximum = 3000000m,
			AmountFormat = "10,3",
			ApiUrl = new("http://localhost/"),
			Version = "1.0",
			MerchantId = "merchantId",
			Deskey = "deskey"
		};
		_ = fakeGameProvider
			.GetVRGamingConfigurationAsync(
			gameProviderConfigurationId: Arg.Is(gameProviderConfigurationId),
			cancellationToken: Arg.Any<CancellationToken>()).
			Returns(ValueTask.FromResult(configuration));

		var gameInfo = new GameInfo(
			GameId: 1,
			GameCode: "test",
			GameProviderId: gameProviderId,
			GameProviderCode: GameProviderType.VRGaming,
			GameExternalId: "0",
			Type: GameType.LotteryLobby,
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
			SubGameProviderCode: GameProviderType.VRGaming);
		_ = fakeGame
			.GetAsync(
				Arg.Is(query.GameId.Value),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameInfo);

		var cultureCode = "en-us";
		_ = fakeGameProvider
			.GetGameProviderCultureCodeAsync(
				gameProviderId: Arg.Is(gameProviderId),
				cultureCode: Arg.Is(query.Language),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(cultureCode);

		var gameurl = "https://localhost";
		_ = fakeClient
			.GameUrlAsync(
			apiUrl: Arg.Is(configuration.ApiUrl),
			version: Arg.Is(configuration.Version),
			merchantId: Arg.Is(configuration.MerchantId),
			deskey: Arg.Is(configuration.Deskey),
			playerCode: Arg.Is(player.PlayerCode),
			externalGameId: Arg.Is(int.Parse(gameInfo.GameExternalId)),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameurl);

		var sut = new VRGamingGameUrlGetService(
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
