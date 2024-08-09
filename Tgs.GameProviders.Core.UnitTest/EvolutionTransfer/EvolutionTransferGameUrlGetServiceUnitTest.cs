using System;
using System.Threading.Tasks;
using Google.Protobuf.Collections;
using NSubstitute;
using Tgs.Core.Infrastructures.Security;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.EvolutionService.Client;
using Tgs.GameInterface.EvolutionService.Protos;
using Tgs.GameProviders.Core.EvolutionTransfer.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.EvolutionTransfer;

public class EvolutionTransferGameUrlGetServiceUnitTest
{
	[Fact]
	public async Task Can_Get_Game_Url_With_Live_Dealer_Lobby()
	{
		var fakeBrandService = Substitute.For<IBrandRepository>();
		var fakeGameRepository = Substitute.For<IGameRepository>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakeGameInterfaceEvolutionServiceGrpc = Substitute.For<IGameInterfaceEvolutionClient>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeSettingRepository = Substitute.For<ISettingRepository>();
		var fakeToken = Substitute.For<ITokenProvider>();

		var configuration = new EvolutionTransferConfiguration
		{
			Ua2AuthenticationUrl = new("https://babylonpega2.uat1.evo-test.com/ua/v1/babylonpega20001/test123"),
			ApiUrl = new("https://babylonpega2.uat1.evo-test.com"),
			CasinoId = "babylonpega20001",
			Ua2Token = "test123",
			GameHistoryApiToken = "test123",
			ExternalLobbyApiToken = "test123",
			EcId = "babylonpega20001test123"
		};

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

		var gameInfo = new GameInfo(
			GameId: 1,
			GameCode: "test",
			GameProviderId: Guid.NewGuid(),
			GameProviderCode: GameProviderType.EvolutionTransfer,
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
			SubGameProviderCode: GameProviderType.EvolutionTransfer);

		var query = new EvolutionTransferGameUrlQuery
		{
			PlayerCode = "4ucmqplxb029",
			GameId = 1,
			LoginRecordId = Guid.NewGuid().ToString(),
			Language = "zh-CN",
			PlatformType = PlatformType.Desktop,
			IPAddress = "127.0.0.1",
			BackUrl = new("https://dev-t1.rpgp.cc/")
		};

		var token = "token";
		var gameProviderCultureCode = "en-US";
		var gameProviderCurrencyCode = "CNY";

		var gameProviderConfigurationId = Guid.NewGuid();
		_ = fakeGameProviderRepository.GetGameProviderIdAsync(Arg.Is(GameProviderType.EvolutionTransfer)).Returns(gameInfo.GameProviderId);
		_ = fakeSettingRepository.GetEvolutionTransferConfigurationAsync().Returns(ValueTask.FromResult(configuration));
		_ = fakePlayerRepository.GetAsync(Arg.Is(query.PlayerCode)).Returns(playerModel);
		_ = fakeBrandService.GetBrandGameProviderConfigurationIdAsync(
			 Arg.Is(gameInfo.GameProviderId),
			 Arg.Is(playerModel.BrandId)).Returns(gameProviderConfigurationId);
		_ = fakeGameProviderRepository.GetGameProviderCultureCodeAsync(
			 Arg.Is(gameInfo.GameProviderId),
			Arg.Is(query.Language)).Returns(gameProviderCultureCode);
		_ = fakeGameRepository.GetAsync(Arg.Is(query.GameId)).Returns(gameInfo);
		_ = fakeToken.EncryptAsync(query.LoginRecordId).Returns(token);
		_ = fakeGameProviderRepository.GetGameProviderCurrencyCodeAsync(
			Arg.Is(gameInfo.GameProviderId),
			Arg.Is(playerModel.DefaultWalletCurrencyCode)).Returns(gameProviderCurrencyCode);

		var response = new GameUrlReply()
		{
			Entry = "/entry",
			EntryEmbedded = "/EntryEmbedded",
			Errors = { new RepeatedField<Error>() }
		};
		_ = fakeGameInterfaceEvolutionServiceGrpc.GetLiveDealerGameUrlAsync(
			 ua2AuthenticationUrl: configuration.Ua2AuthenticationUrl,
			 brandCode: playerModel.BrandCode,
			 playerCode: playerModel.PlayerCode,
			 platformType: (short)query.PlatformType,
			 ipAddress: query.IPAddress,
			 encryptedToken: token,
			 externalCultureCode: gameProviderCultureCode,
			 externalCurrencyCode: playerModel.DefaultWalletCurrencyCode,
			 cancellationToken: default
		).Returns(response);

		var sut = new EvolutionTransferGameUrlGetService(
			fakeGameProviderRepository,
			fakeGameRepository,
			fakeGameInterfaceEvolutionServiceGrpc,
			fakePlayerRepository,
			fakeSettingRepository,
			fakeToken);

		var actual = await sut.GetAsync(query);

		var path = response.Entry[1..];

		Assert.NotNull(actual);
		Assert.Equal($"{configuration.ApiUrl}{path}", actual);
	}

	[Fact]
	public async Task Normal_Process()
	{
		var fakeBrandService = Substitute.For<IBrandRepository>();
		var fakeGameRepository = Substitute.For<IGameRepository>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakeGameInterfaceEvolutionServiceGrpc = Substitute.For<IGameInterfaceEvolutionClient>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeSettingRepository = Substitute.For<ISettingRepository>();
		var fakeToken = Substitute.For<ITokenProvider>();

		var configuration = new EvolutionTransferConfiguration
		{
			Ua2AuthenticationUrl = new("https://babylonpega2.uat1.evo-test.com/ua/v1/babylonpega20001/test123"),
			ApiUrl = new("https://babylonpega2.uat1.evo-test.com"),
			CasinoId = "babylonpega20001",
			Ua2Token = "test123",
			GameHistoryApiToken = "test123",
			ExternalLobbyApiToken = "test123",
			EcId = "babylonpega20001test123"
		};

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

		var gameInfo = new GameInfo(
			GameId: 1,
			GameCode: "test",
			GameProviderId: Guid.NewGuid(),
			GameProviderCode: GameProviderType.EvolutionTransfer,
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
			SubGameProviderCode: GameProviderType.EvolutionTransfer);

		var query = new EvolutionTransferGameUrlQuery
		{
			PlayerCode = "4ucmqplxb029",
			GameId = 1,
			LoginRecordId = Guid.NewGuid().ToString(),
			Language = "zh-CN",
			PlatformType = PlatformType.Desktop,
			IPAddress = "127.0.0.1",
			BackUrl = new("https://dev-t1.rpgp.cc/")
		};

		var token = "token";
		var gameProviderCultureCode = "en-US";
		var gameProviderCurrencyCode = "CNY";

		var gameProviderConfigurationId = Guid.NewGuid();
		_ = fakeGameProviderRepository.GetGameProviderIdAsync(Arg.Is(GameProviderType.EvolutionTransfer)).Returns(gameInfo.GameProviderId);
		_ = fakeSettingRepository.GetEvolutionTransferConfigurationAsync().Returns(ValueTask.FromResult(configuration));
		_ = fakePlayerRepository.GetAsync(Arg.Is(query.PlayerCode)).Returns(playerModel);
		_ = fakeBrandService.GetBrandGameProviderConfigurationIdAsync(
			 Arg.Is(gameInfo.GameProviderId),
			 Arg.Is(playerModel.BrandId)).Returns(gameProviderConfigurationId);
		_ = fakeGameProviderRepository.GetGameProviderCultureCodeAsync(
			 Arg.Is(gameInfo.GameProviderId),
			Arg.Is(query.Language)).Returns(gameProviderCultureCode);
		_ = fakeGameRepository.GetAsync(Arg.Is(query.GameId)).Returns(gameInfo);
		_ = fakeToken.EncryptAsync(query.LoginRecordId).Returns(token);
		_ = fakeGameProviderRepository.GetGameProviderCurrencyCodeAsync(
			Arg.Is(gameInfo.GameProviderId),
			Arg.Is(playerModel.DefaultWalletCurrencyCode)).Returns(gameProviderCurrencyCode);

		var response = new GameUrlReply()
		{
			Entry = "/entry",
			EntryEmbedded = "/EntryEmbedded",
			Errors = { new RepeatedField<Error>() }
		};
		_ = fakeGameInterfaceEvolutionServiceGrpc.GetSlotGameUrlAsync(
			 ua2AuthenticationUrl: configuration.Ua2AuthenticationUrl,
			 brandCode: playerModel.BrandCode,
			 playerCode: playerModel.PlayerCode,
			 platformType: (short)query.PlatformType,
			 ipAddress: query.IPAddress,
			 encryptedToken: token,
			 externalGameId: gameInfo.GameExternalId,
			 externalCultureCode: gameProviderCultureCode,
			 externalCurrencyCode: playerModel.DefaultWalletCurrencyCode,
			 cancellationToken: default
		).Returns(response);

		var sut = new EvolutionTransferGameUrlGetService(
			fakeGameProviderRepository,
			fakeGameRepository,
			fakeGameInterfaceEvolutionServiceGrpc,
			fakePlayerRepository,
			fakeSettingRepository,
			fakeToken);

		var actual = await sut.GetAsync(query);

		var path = response.Entry[1..];

		Assert.NotNull(actual);
		Assert.Equal($"{configuration.ApiUrl}{path}", actual);
	}
}
