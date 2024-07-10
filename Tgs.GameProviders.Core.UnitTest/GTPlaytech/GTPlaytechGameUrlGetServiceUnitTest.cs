using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.GlobalTechnology.Client;
using Tgs.GameInterface.GlobalTechnology.Protos;
using Tgs.GameProviders.Core.GTPlaytech.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.GTPlaytech;

public class GTPlaytechGameUrlGetServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeBrandRepository = Substitute.For<IBrandRepository>();
		var fakeGameRepository = Substitute.For<IGameRepository>();
		var fakeClient = Substitute.For<IGameInterfaceGlobalTechnologyClient>();

		var query = new GTPlaytechGameUrlQuery
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
			gameProviderCode: Arg.Is(GameProviderType.GT_Playtech),
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

		var configuration = new GTPlaytechConfiguration()
		{
			ApiUrl = new("https://api-stg.gmgiantgold.com/"),
			LauncherApiUrl = new("https://api-stg.gmgiantgold.com/"),
			DataApiUrl = new("http://gm-stage-data.gmgiantgold.com/"),
			BrandId = 911,
			SecretKey = "pPRHm1NHFosSOUv4tVjv"
		};
		_ = fakeGameProviderRepository.GetGTPlaytechConfigurationAsync(
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
			GameProviderCode: GameProviderType.GT_Playtech,
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
			SubGameProviderCode: GameProviderType.GT_Playtech);
		_ = fakeGameRepository.GetAsync(
			gameId: Arg.Is(query.GameId.Value),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(game);

		var reply = new GameUrlReply
		{
			IsPlayerExist = true,
			GameUrl = "https://api-stg.gmgiantgold.com/Launcher"
		};

		_ = fakeClient.GameUrlAsync(
			apiUrl: Arg.Is(configuration.ApiUrl),
			launcherApiUrl: Arg.Is(configuration.LauncherApiUrl),
			brandId: Arg.Is(configuration.BrandId),
			secretKey: Arg.Is(configuration.SecretKey),
			playerCode: Arg.Is(player.PlayerCode),
			currency: Arg.Is(currency),
			language: Arg.Is(cultureCode),
			isMobile: Arg.Any<bool>(),
			gameCode: Arg.Is(game.GameExternalId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(reply);

		var sut = new GTPlaytechGameUrlGetService(
			fakeGameProviderRepository,
			fakePlayerRepository,
			fakeBrandRepository,
			fakeGameRepository,
			fakeClient);

		var actual = await sut.GetAsync(query);

		Assert.NotNull(actual);
		Assert.NotEmpty(actual);

		_ = fakeClient
			.Received(0)
			.GameUrlPlaytechByLiveAsync(
			apiUrl: Arg.Is(configuration.ApiUrl),
			launcherApiUrl: Arg.Is(configuration.LauncherApiUrl),
			brandId: Arg.Is(configuration.BrandId),
			secretKey: Arg.Is(configuration.SecretKey),
			playerCode: Arg.Is(player.PlayerCode),
			currency: Arg.Is(currency),
			language: Arg.Is(cultureCode),
			isMobile: Arg.Any<bool>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeClient
			.Received(0)
			.PlayerCreateAsync(
			apiUrl: Arg.Is(configuration.ApiUrl),
			brandId: Arg.Is(configuration.BrandId),
			secretKey: Arg.Is(configuration.SecretKey),
			playerCode: Arg.Is(player.PlayerCode),
			currency: Arg.Is(currency),
			language: Arg.Is(cultureCode),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task LiveDealerLobby()
	{
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeBrandRepository = Substitute.For<IBrandRepository>();
		var fakeGameRepository = Substitute.For<IGameRepository>();
		var fakeClient = Substitute.For<IGameInterfaceGlobalTechnologyClient>();

		var query = new GTPlaytechGameUrlQuery
		{
			GameType = GameType.LiveDealerLobby,
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
			gameProviderCode: Arg.Is(GameProviderType.GT_Playtech),
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

		var configuration = new GTPlaytechConfiguration()
		{
			ApiUrl = new("https://api-stg.gmgiantgold.com/"),
			LauncherApiUrl = new("https://api-stg.gmgiantgold.com/"),
			DataApiUrl = new("http://gm-stage-data.gmgiantgold.com/"),
			BrandId = 911,
			SecretKey = "pPRHm1NHFosSOUv4tVjv"
		};
		_ = fakeGameProviderRepository.GetGTPlaytechConfigurationAsync(
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
			GameProviderCode: GameProviderType.GT_Playtech,
			GameExternalId: "1",
			Type: GameType.LiveDealerLobby,
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
			SubGameProviderCode: GameProviderType.GT_Playtech);
		_ = fakeGameRepository.GetAsync(
			gameId: Arg.Is(query.GameId.Value),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(game);

		var reply = new GameUrlReply
		{
			IsPlayerExist = true,
			GameUrl = "https://api-stg.gmgiantgold.com/Launcher"
		};

		_ = fakeClient.GameUrlPlaytechByLiveAsync(
			apiUrl: Arg.Is(configuration.ApiUrl),
			launcherApiUrl: Arg.Is(configuration.LauncherApiUrl),
			brandId: Arg.Is(configuration.BrandId),
			secretKey: Arg.Is(configuration.SecretKey),
			playerCode: Arg.Is(player.PlayerCode),
			currency: Arg.Is(currency),
			language: Arg.Is(cultureCode),
			isMobile: Arg.Any<bool>(),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(reply);

		var sut = new GTPlaytechGameUrlGetService(
			fakeGameProviderRepository,
			fakePlayerRepository,
			fakeBrandRepository,
			fakeGameRepository,
			fakeClient);

		var actual = await sut.GetAsync(query);

		Assert.NotNull(actual);
		Assert.NotEmpty(actual);

		_ = fakeClient
			.Received(0)
			.GameUrlAsync(
			apiUrl: Arg.Is(configuration.ApiUrl),
			launcherApiUrl: Arg.Is(configuration.LauncherApiUrl),
			brandId: Arg.Is(configuration.BrandId),
			secretKey: Arg.Is(configuration.SecretKey),
			playerCode: Arg.Is(player.PlayerCode),
			currency: Arg.Is(currency),
			language: Arg.Is(cultureCode),
			isMobile: Arg.Any<bool>(),
			gameCode: Arg.Is(game.GameExternalId),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeClient
			.Received(0)
			.PlayerCreateAsync(
			apiUrl: Arg.Is(configuration.ApiUrl),
			brandId: Arg.Is(configuration.BrandId),
			secretKey: Arg.Is(configuration.SecretKey),
			playerCode: Arg.Is(player.PlayerCode),
			currency: Arg.Is(currency),
			language: Arg.Is(cultureCode),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task Player_does_not_exist()
	{
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeBrandRepository = Substitute.For<IBrandRepository>();
		var fakeGameRepository = Substitute.For<IGameRepository>();
		var fakeClient = Substitute.For<IGameInterfaceGlobalTechnologyClient>();

		var query = new GTPlaytechGameUrlQuery
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
			gameProviderCode: Arg.Is(GameProviderType.GT_Playtech),
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

		var configuration = new GTPlaytechConfiguration()
		{
			ApiUrl = new("https://api-stg.gmgiantgold.com/"),
			LauncherApiUrl = new("https://api-stg.gmgiantgold.com/"),
			DataApiUrl = new("http://gm-stage-data.gmgiantgold.com/"),
			BrandId = 911,
			SecretKey = "pPRHm1NHFosSOUv4tVjv"
		};
		_ = fakeGameProviderRepository.GetGTPlaytechConfigurationAsync(
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
			GameProviderCode: GameProviderType.GT_Playtech,
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
			SubGameProviderCode: GameProviderType.GT_Playtech);
		_ = fakeGameRepository.GetAsync(
			gameId: Arg.Is(query.GameId.Value),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(game);

		_ = fakeClient.GameUrlAsync(
			apiUrl: Arg.Is(configuration.ApiUrl),
			launcherApiUrl: Arg.Is(configuration.LauncherApiUrl),
			brandId: Arg.Is(configuration.BrandId),
			secretKey: Arg.Is(configuration.SecretKey),
			playerCode: Arg.Is(player.PlayerCode),
			currency: Arg.Is(currency),
			language: Arg.Is(cultureCode),
			isMobile: Arg.Any<bool>(),
			gameCode: Arg.Is(game.GameExternalId),
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
				  GameUrl = "https://api-stg.gmgiantgold.com/Launcher"
			  });

		var sut = new GTPlaytechGameUrlGetService(
			fakeGameProviderRepository,
			fakePlayerRepository,
			fakeBrandRepository,
			fakeGameRepository,
			fakeClient);

		var actual = await sut.GetAsync(query);

		Assert.NotNull(actual);
		Assert.NotEmpty(actual);

		_ = fakeClient
			.Received(1)
			.PlayerCreateAsync(
			apiUrl: Arg.Is(configuration.ApiUrl),
			brandId: Arg.Is(configuration.BrandId),
			secretKey: Arg.Is(configuration.SecretKey),
			playerCode: Arg.Is(player.PlayerCode),
			currency: Arg.Is(currency),
			language: Arg.Is(cultureCode),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
