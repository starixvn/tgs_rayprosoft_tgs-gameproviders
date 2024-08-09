using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.PragmaticPlay.Client;
using Tgs.GameInterface.PragmaticPlay.Protos;
using Tgs.GameProviders.Core.PragmaticPlayTransfer.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.PragmaticPlayTransfer;

public class PragmaticPlayTransferGameUrlGetServiceUnitTest
{
	[Fact]
	public async Task Create_PlayerAsync()
	{
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeBrandRepository = Substitute.For<IBrandRepository>();
		var fakeGameRepository = Substitute.For<IGameRepository>();
		var fakeClient = Substitute.For<IGameInterfacePragmaticPlayClient>();

		var query = new PragmaticPlayTransferGameUrlQuery
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
		_ = fakeGameProviderRepository
			.GetGameProviderIdAsync(
				gameProviderCode: Arg.Is(GameProviderType.PragmaticPlayTransfer),
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
		_ = fakePlayerRepository
			.GetAsync(
				playerCode: Arg.Is(query.PlayerCode),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(player);

		var gameProviderConfigurationId = Guid.NewGuid();
		_ = fakeBrandRepository
			.GetBrandGameProviderConfigurationIdAsync(
				 gameProviderId: Arg.Is(gameProviderId),
				 brandId: Arg.Is(player.BrandId),
				 cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameProviderConfigurationId);

		var configuration = new PragmaticPlayTransferConfiguration()
		{
			ApiUrl = new("https://localhost/"),
			SecureLogin = "secure_login",
			SecretKey = "secret_key"
		};
		_ = fakeGameProviderRepository
			.GetPragmaticPlayTransferConfigurationAsync(
				gameProviderConfigurationId: Arg.Is(gameProviderConfigurationId),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(configuration);

		var cultureCode = "en";
		_ = fakeGameProviderRepository
			.GetGameProviderCultureCodeAsync(
				gameProviderId: Arg.Is(gameProviderId),
				cultureCode: Arg.Is(query.Language),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(cultureCode);

		var game = new GameInfo(
			GameId: 1,
			GameCode: "test",
			GameProviderId: Guid.NewGuid(),
			GameProviderCode: GameProviderType.PragmaticPlayTransfer,
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
			SubGameProviderCode: GameProviderType.PragmaticPlayTransfer);
		_ = fakeGameRepository.GetAsync(
			gameId: Arg.Is(query.GameId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(game);

		var gameUrlReply = new GameUrlReply
		{
			IsPlayerExist = false,
			GameUrl = string.Empty
		};
		_ = fakeClient
			.GameUrlAsync(
				apiUrl: Arg.Is(configuration.ApiUrl),
				secureLogin: Arg.Is(configuration.SecureLogin),
				secretKey: Arg.Is(configuration.SecretKey),
				playerCode: Arg.Is(query.PlayerCode),
				externalGameId: Arg.Is(game.GameExternalId),
				language: Arg.Is(cultureCode),
				isMobile: Arg.Is(query.PlatformType == PlatformType.Mobile),
				lobbyUrl: Arg.Is(query.BackUrl?.OriginalString),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameUrlReply);

		var currencyCode = "CNY";
		_ = fakeGameProviderRepository
			.GetGameProviderCurrencyCodeAsync(
				gameProviderId: gameProviderId,
				currencyCode: player.DefaultWalletCurrencyCode,
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(currencyCode);

		var sut = new PragmaticPlayTransferGameUrlGetService(
			fakeGameProviderRepository,
			fakePlayerRepository,
			fakeBrandRepository,
			fakeGameRepository,
			fakeClient);

		_ = await Assert.ThrowsAsync<Exception>(() => sut.GetAsync(query: query).AsTask());

		_ = fakeClient
			.Received(1)
			.PlayerCreateAsync(
				apiUrl: Arg.Is(configuration.ApiUrl),
				secureLogin: Arg.Is(configuration.SecureLogin),
				secretKey: Arg.Is(configuration.SecretKey),
				playerCode: Arg.Is(query.PlayerCode),
				currencyCode: Arg.Is(currencyCode),
				Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task Normal_Process()
	{
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeBrandRepository = Substitute.For<IBrandRepository>();
		var fakeGameRepository = Substitute.For<IGameRepository>();
		var fakeClient = Substitute.For<IGameInterfacePragmaticPlayClient>();

		var query = new PragmaticPlayTransferGameUrlQuery
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
		_ = fakeGameProviderRepository
			.GetGameProviderIdAsync(
				gameProviderCode: Arg.Is(GameProviderType.PragmaticPlayTransfer),
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
		_ = fakePlayerRepository
			.GetAsync(
				playerCode: Arg.Is(query.PlayerCode),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(player);

		var gameProviderConfigurationId = Guid.NewGuid();
		_ = fakeBrandRepository
			.GetBrandGameProviderConfigurationIdAsync(
				 gameProviderId: Arg.Is(gameProviderId),
				 brandId: Arg.Is(player.BrandId),
				 cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameProviderConfigurationId);

		var configuration = new PragmaticPlayTransferConfiguration()
		{
			ApiUrl = new("https://localhost/"),
			SecureLogin = "secure_login",
			SecretKey = "secret_key"
		};
		_ = fakeGameProviderRepository
			.GetPragmaticPlayTransferConfigurationAsync(
				gameProviderConfigurationId: Arg.Is(gameProviderConfigurationId),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(configuration);

		var cultureCode = "en";
		_ = fakeGameProviderRepository
			.GetGameProviderCultureCodeAsync(
				gameProviderId: Arg.Is(gameProviderId),
				cultureCode: Arg.Is(query.Language),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(cultureCode);

		var game = new GameInfo(
			GameId: 1,
			GameCode: "test",
			GameProviderId: Guid.NewGuid(),
			GameProviderCode: GameProviderType.PragmaticPlayTransfer,
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
			SubGameProviderCode: GameProviderType.PragmaticPlayTransfer);
		_ = fakeGameRepository	.GetAsync(
			gameId: Arg.Is(query.GameId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(game);

		var gameUrlReply = new GameUrlReply
		{
			IsPlayerExist = true,
			GameUrl = "https://localhost/"
		};
		_ = fakeClient
			.GameUrlAsync(
				apiUrl: Arg.Is(configuration.ApiUrl),
				secureLogin: Arg.Is(configuration.SecureLogin),
				secretKey: Arg.Is(configuration.SecretKey),
				playerCode: Arg.Is(query.PlayerCode),
				externalGameId: Arg.Is(game.GameExternalId),
				language: Arg.Is(cultureCode),
				isMobile: Arg.Is(query.PlatformType == PlatformType.Mobile),
				lobbyUrl: Arg.Is(query.BackUrl?.OriginalString),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameUrlReply);

		var currencyCode = "CNY";
		_ = fakeGameProviderRepository
			.GetGameProviderCurrencyCodeAsync(
				gameProviderId: gameProviderId,
				currencyCode: player.DefaultWalletCurrencyCode,
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(currencyCode);

		var sut = new PragmaticPlayTransferGameUrlGetService(
			fakeGameProviderRepository,
			fakePlayerRepository,
			fakeBrandRepository,
			fakeGameRepository,
			fakeClient);

		var actual = await sut.GetAsync(query);

		Assert.NotNull(actual);
		Assert.Equal(gameUrlReply.GameUrl, actual);
	}
}
