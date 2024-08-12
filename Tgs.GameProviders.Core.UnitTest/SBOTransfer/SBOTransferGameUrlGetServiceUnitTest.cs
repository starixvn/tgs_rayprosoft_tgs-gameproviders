using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.SBOService.Client;
using Tgs.GameInterface.SBOService.Protos;
using Tgs.GameProviders.Core.SBOTransfer.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.SBOTransfer;

public class SBOTransferGameUrlGetServiceUnitTest
{
	[Fact]
	public async Task Normal_Process_SportsBook()
	{
		var fakeGameInterfaceSBOClient = Substitute.For<IGameInterfaceSBOClient>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeBrandService = Substitute.For<IBrandRepository>();
		var fakeGameRepository = Substitute.For<IGameRepository>();

		var query = new SBOTransferGameUrlQuery
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
		_ = fakeGameProviderRepository.GetGameProviderIdAsync(Arg.Is(GameProviderType.SBOTransfer)).Returns(gameProviderId);

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
		_ = fakePlayerRepository.GetAsync(Arg.Is(query.PlayerCode)).Returns(player);

		var gameProviderConfigurationId = Guid.NewGuid();
		_ = fakeBrandService.GetBrandGameProviderConfigurationIdAsync(
			 Arg.Is(gameProviderId),
			 Arg.Is(player.BrandId)).Returns(gameProviderConfigurationId);

		var configuration = new SBOTransferConfiguration()
		{
			ApiUrl = new("http://localhost/"),
			CompanyKey = "companyKey",
			Agent = "Agent",
			Currency = "CNY",
			Min = 1,
			Max = 5000,
			MaxPerMatch = 20000,
			CasinoTableLimit = 1
		};
		_ = fakeGameProviderRepository.GetSBOTransferConfigurationAsync(Arg.Is(gameProviderConfigurationId)).Returns(configuration);

		var gameInfo = new GameInfo(
			GameId: 1,
			GameCode: "test",
			GameProviderId: Guid.NewGuid(),
			GameProviderCode: GameProviderType.SBOTransfer,
			GameExternalId: "1",
			Type: GameType.Sportsbook,
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
			SubGameProviderCode: GameProviderType.SBOTransfer);
		_ = fakeGameRepository.GetAsync(
			gameId: Arg.Is(query.GameId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameInfo);

		var gameProviderCultureCode = "en";
		_ = fakeGameProviderRepository.GetGameProviderCultureCodeAsync(
			 Arg.Is(gameProviderId),
			 Arg.Is(query.Language)).Returns(gameProviderCultureCode);

		var response = new GameUrlReply
		{
			IsPlayerExist = true,
			GameUrl = "http://localhost/"
		};
		_ = fakeGameInterfaceSBOClient.GameUrlBySportsBookAsync(
				Arg.Is(configuration.ApiUrl),
				Arg.Is(configuration.CompanyKey),
				Arg.Is(configuration.Agent),
				Arg.Is(player.PlayerCode),
				Arg.Is(gameProviderCultureCode),
				Arg.Is(false)).Returns(response);

		var sut = new SBOTransferGameUrlGetService(
			fakeGameInterfaceSBOClient,
			fakeGameProviderRepository,
			fakePlayerRepository,
			fakeBrandService,
			fakeGameRepository);

		var actual = await sut.GetAsync(query);

		Assert.NotNull(actual);
		Assert.Equal("http://localhost/", actual);
	}

	[Fact]
	public async Task Normal_Process_VirtualSports()
	{
		var fakeGameInterfaceSBOClient = Substitute.For<IGameInterfaceSBOClient>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeBrandService = Substitute.For<IBrandRepository>();
		var fakeGameRepository = Substitute.For<IGameRepository>();

		var query = new SBOTransferGameUrlQuery
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
		_ = fakeGameProviderRepository.GetGameProviderIdAsync(Arg.Is(GameProviderType.SBOTransfer)).Returns(gameProviderId);

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
		_ = fakePlayerRepository.GetAsync(Arg.Is(query.PlayerCode)).Returns(player);

		var gameProviderConfigurationId = Guid.NewGuid();
		_ = fakeBrandService.GetBrandGameProviderConfigurationIdAsync(
			 Arg.Is(gameProviderId),
			 Arg.Is(player.BrandId)).Returns(gameProviderConfigurationId);

		var configuration = new SBOTransferConfiguration()
		{
			ApiUrl = new("http://localhost/"),
			CompanyKey = "companyKey",
			Agent = "Agent",
			Currency = "CNY",
			Min = 1,
			Max = 5000,
			MaxPerMatch = 20000,
			CasinoTableLimit = 1
		};
		_ = fakeGameProviderRepository.GetSBOTransferConfigurationAsync(Arg.Is(gameProviderConfigurationId)).Returns(configuration);

		var gameInfo = new GameInfo(
			GameId: 1,
			GameCode: "test",
			GameProviderId: Guid.NewGuid(),
			GameProviderCode: GameProviderType.SBOTransfer,
			GameExternalId: "7_1",
			Type: GameType.VirtualSportsLobby,
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
			SubGameProviderCode: GameProviderType.SBOTransfer);
		_ = fakeGameRepository.GetAsync(
			gameId: Arg.Is(query.GameId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameInfo);

		var gameProviderCultureCode = "en";
		_ = fakeGameProviderRepository.GetGameProviderCultureCodeAsync(
			 Arg.Is(gameProviderId),
			 Arg.Is(query.Language)).Returns(gameProviderCultureCode);

		var response = new GameUrlReply
		{
			IsPlayerExist = true,
			GameUrl = "http://localhost/"
		};
		_ = fakeGameInterfaceSBOClient.GameUrlByVirtualSportsAsync(
				Arg.Is(configuration.ApiUrl),
				Arg.Is(configuration.CompanyKey),
				Arg.Is(configuration.Agent),
				Arg.Is(player.PlayerCode),
				Arg.Is(gameProviderCultureCode)).Returns(response);

		var sut = new SBOTransferGameUrlGetService(
			fakeGameInterfaceSBOClient,
			fakeGameProviderRepository,
			fakePlayerRepository,
			fakeBrandService,
			fakeGameRepository);

		var actual = await sut.GetAsync(query);

		Assert.NotNull(actual);
		Assert.Equal("http://localhost/", actual);
	}
}
