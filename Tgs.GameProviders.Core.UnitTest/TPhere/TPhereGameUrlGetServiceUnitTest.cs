using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.TPhere.Client;
using Tgs.GameInterface.TPhere.Protos;
using Tgs.GameProviders.Core.TPhere.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.TPhere;

public class TPhereGameUrlGetServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeBrandRepository = Substitute.For<IBrandRepository>();
		var fakeGameRepository = Substitute.For<IGameRepository>();
		var fakeClient = Substitute.For<IGameInterfaceTPhereClient>();

		var query = new TPhereGameUrlQuery
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
			gameProviderCode: Arg.Is(GameProviderType.TPhere),
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

		var configuration = new TPhereConfiguration()
		{
			ApiUrl = new("http://trial-gp-api.funkytest.com/"),
			ApiKey = "6309572cf90658e17847a7b021723106",
			ApiToken = "29dad28e8ab9c9855e6aa4b9a9e4a869325931d4976dfcf53266ccbe0c5f3387ea48258c002e3543edbb35f63caa5cacc65ac5c664ccfe0619e1d63018a87dbb",
		};
		_ = fakeGameProviderRepository.GetTPhereConfigurationAsync(
			gameProviderConfigurationId: Arg.Is(gameProviderConfigurationId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(configuration);

		var currency = "CNY";
		_ = fakeGameProviderRepository.GetGameProviderCurrencyCodeAsync(
			gameProviderId: Arg.Is(gameProviderId),
			currencyCode: Arg.Is(player.DefaultWalletCurrencyCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(currency);

		var cultureCode = "EN";
		_ = fakeGameProviderRepository.GetGameProviderCultureCodeAsync(
			gameProviderId: Arg.Is(gameProviderId),
			cultureCode: Arg.Is(query.Language),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(cultureCode);

		var game = new GameInfo(
			GameId: 1,
			GameCode: "test",
			GameProviderId: Guid.NewGuid(),
			GameProviderCode: GameProviderType.TPhere,
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
			SubGameProviderCode: GameProviderType.TPhere);
		_ = fakeGameRepository.GetAsync(
			gameId: Arg.Is(query.GameId.Value),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(game);

		var reply = new GameUrlReply
		{
			IsPlayerExist = true,
			GameUrl = "https:\\/\\/m-api.yahutech.com\\/Login"
		};

		_ = fakeClient.TP2GameUrlAsync(
			apiUrl: Arg.Is(configuration.ApiUrl),
			apiKey: Arg.Is(configuration.ApiKey),
			apiToken: Arg.Is(configuration.ApiToken),
			playerCode: Arg.Is(player.PlayerCode),
			gameCode: Arg.Is(game.GameExternalId),
			isMobile: Arg.Any<bool>(),
			language: Arg.Is(cultureCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(reply);

		var sut = new TPhereGameUrlGetService(
			fakeGameProviderRepository,
			fakePlayerRepository,
			fakeBrandRepository,
			fakeGameRepository,
			fakeClient);

		var actual = await sut.GetAsync(query);

		Assert.NotNull(actual);
		Assert.NotEmpty(actual);
	}

	[Fact]
	public async Task Player_does_not_exist()
	{
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeBrandRepository = Substitute.For<IBrandRepository>();
		var fakeGameRepository = Substitute.For<IGameRepository>();
		var fakeClient = Substitute.For<IGameInterfaceTPhereClient>();

		var query = new TPhereGameUrlQuery
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
			gameProviderCode: Arg.Is(GameProviderType.TPhere),
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

		var configuration = new TPhereConfiguration()
		{
			ApiUrl = new("http://trial-gp-api.funkytest.com/"),
			ApiKey = "6309572cf90658e17847a7b021723106",
			ApiToken = "29dad28e8ab9c9855e6aa4b9a9e4a869325931d4976dfcf53266ccbe0c5f3387ea48258c002e3543edbb35f63caa5cacc65ac5c664ccfe0619e1d63018a87dbb",
		};
		_ = fakeGameProviderRepository.GetTPhereConfigurationAsync(
			gameProviderConfigurationId: Arg.Is(gameProviderConfigurationId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(configuration);

		var currency = "CNY";
		_ = fakeGameProviderRepository.GetGameProviderCurrencyCodeAsync(
			gameProviderId: Arg.Is(gameProviderId),
			currencyCode: Arg.Is(player.DefaultWalletCurrencyCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(currency);

		var cultureCode = "EN";
		_ = fakeGameProviderRepository.GetGameProviderCultureCodeAsync(
			gameProviderId: Arg.Is(gameProviderId),
			cultureCode: Arg.Is(query.Language),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(cultureCode);

		var game = new GameInfo(
			GameId: 1,
			GameCode: "test",
			GameProviderId: Guid.NewGuid(),
			GameProviderCode: GameProviderType.TPhere,
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
			SubGameProviderCode: GameProviderType.TPhere);
		_ = fakeGameRepository.GetAsync(
			gameId: Arg.Is(query.GameId.Value),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(game);

		_ = fakeClient.TP2GameUrlAsync(
			apiUrl: Arg.Is(configuration.ApiUrl),
			apiKey: Arg.Is(configuration.ApiKey),
			apiToken: Arg.Is(configuration.ApiToken),
			playerCode: Arg.Is(player.PlayerCode),
			gameCode: Arg.Is(game.GameExternalId),
			isMobile: Arg.Any<bool>(),
			language: Arg.Is(cultureCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(
			  new GameUrlReply
			  {
				  IsPlayerExist = false,
				  GameUrl = string.Empty,
			  },
			  new GameUrlReply
			  {
				  IsPlayerExist = true,
				  GameUrl = "https:\\/\\/m-api.yahutech.com\\/Login",
			  });

		var sut = new TPhereGameUrlGetService(
			fakeGameProviderRepository,
			fakePlayerRepository,
			fakeBrandRepository,
			fakeGameRepository,
			fakeClient);

		var actual = await sut.GetAsync(query);

		Assert.NotNull(actual);
		Assert.NotEmpty(actual);

		await fakeClient
			.Received(1)
			.TP2PlayerCreateAsync(
			apiUrl: Arg.Is(configuration.ApiUrl),
			apiKey: Arg.Is(configuration.ApiKey),
			apiToken: Arg.Is(configuration.ApiToken),
			playerCode: Arg.Is(player.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
