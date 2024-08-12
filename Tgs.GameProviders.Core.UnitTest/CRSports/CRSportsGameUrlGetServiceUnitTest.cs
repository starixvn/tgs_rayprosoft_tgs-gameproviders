using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.CRSports.Client;
using Tgs.GameInterface.CRSports.Protos;
using Tgs.GameProviders.Core.CRSports.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.CRSports;

public class CRSportsGameUrlGetServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeBrandRepository = Substitute.For<IBrandRepository>();
		var fakeGameRepository = Substitute.For<IGameRepository>();
		var fakeClient = Substitute.For<IGameInterfaceCRSportsClient>();

		var query = new CRSportsGameUrlQuery
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
		_ = fakeGameProviderRepository.GetGameProviderIdAsync(
			gameProviderCode: Arg.Is(GameProviderType.CRSports),
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

		var configuration = new CRSportsConfiguration
		{
			ApiUrl = new("https://localhost/"),
			AgentId = 33,
			SecretKey = "aecretKey",
			AgentAccount = "agentAccount",
			AgentPassword = "agentPassword",
		};
		_ = fakeGameProviderRepository.GetCRSportsConfigurationAsync(
			gameProviderConfigurationId: Arg.Is(gameProviderConfigurationId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(configuration);

		var cultureCode = "en-us";
		_ = fakeGameProviderRepository.GetGameProviderCultureCodeAsync(
			gameProviderId: Arg.Is(gameProviderId),
			cultureCode: Arg.Is(query.Language),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(cultureCode);

		var currencyCode = "CYN";
		_ = fakeGameProviderRepository.GetGameProviderCurrencyCodeAsync(
			gameProviderId: Arg.Is(gameProviderId),
			currencyCode: Arg.Is(player.DefaultWalletCurrencyCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(currencyCode);

		var game = new GameInfo(
			GameId: 1,
			GameCode: "test",
			GameProviderId: Guid.NewGuid(),
			GameProviderCode: GameProviderType.CRSports,
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
			gameId: Arg.Is(query.GameId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(game);

		var gameLoginReply = new GameUrlReply
		{
			GameUrl = "http://localhost/"
		};

		_ = fakeClient.GameUrlAsync(
			apiUrl: Arg.Is(configuration.ApiUrl),
			agentId: Arg.Is(configuration.AgentId),
			secretKey: Arg.Is(configuration.SecretKey),
			agentAccount: Arg.Is(configuration.AgentAccount),
			agentPassword: Arg.Is(configuration.AgentPassword),
			playerCode: Arg.Is(player.PlayerCode),
			currency: Arg.Is(currencyCode),
			language: Arg.Is(cultureCode),
			isMobile: Arg.Any<bool>(),
			ipAddress: Arg.Is(query.IPAddress),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameLoginReply);

		var sut = new CRSportsGameUrlGetService(
			fakeGameProviderRepository,
			fakePlayerRepository,
			fakeBrandRepository,
			fakeClient);

		var actual = await sut.GetAsync(query);

		Assert.NotNull(actual);
		Assert.NotEmpty(actual.ToString());
	}
}
