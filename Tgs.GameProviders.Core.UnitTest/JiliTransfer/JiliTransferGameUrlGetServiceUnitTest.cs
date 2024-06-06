using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.Jili.Client;
using Tgs.GameInterface.Jili.Protos;
using Tgs.GameProviders.Core.JiliTransfer.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.UnitTest.JiliTransfer;

public class JiliTransferGameUrlGetServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeBrandService = Substitute.For<IBrandRepository>();
		var fakeGameRepository = Substitute.For<IGameRepository>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakeClient = Substitute.For<IGameInterfaceJiliClient>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();

		var query = new JiliTransferGameUrlQuery
		{
			GameType = GameType.Slot,
			GameId = 1,
			BrandCode = "GPI",
			PlayerCode = "4ucmqplxb029",
			CurrencyCode = "CNY",
			LoginRecordId = Guid.NewGuid(),
			PlatformType = PlatformType.Desktop,
			Language = "en-US",
			IPAddress = "127.0.0.1",
			BackUrl = new("https://dev-t1.rpgp.cc/")
		};

		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProviderRepository.GetGameProviderIdAsync(
			Arg.Is(GameProviderType.JiliTransfer),
			Arg.Any<CancellationToken>())
			.Returns(gameProviderId);

		var playerInfo = new PlayerInfo(
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
			Arg.Is(query.PlayerCode),
			Arg.Any<CancellationToken>())
			.Returns(playerInfo);

		var gameProviderConfigurationId = Guid.NewGuid();
		_ = fakeBrandService.GetBrandGameProviderConfigurationIdAsync(
			Arg.Is(gameProviderId),
			Arg.Is(playerInfo.BrandId),
			Arg.Any<CancellationToken>())
			.Returns(gameProviderConfigurationId);

		var configuration = new JiliTransferConfiguration
		{
			ApiUrl = new("https://tttint.onlinegames22.com/"),
			AgentKey = Guid.NewGuid().ToString("N"),
			AgentId = "testAgentId"
		};
		_ = fakeGameProviderRepository.GetJiliTransferConfigurationAsync(
			Arg.Is(gameProviderConfigurationId),
			Arg.Any<CancellationToken>())
			.Returns(configuration);

		var gameProviderCultureCode = "en-US";
		_ = fakeGameProviderRepository.GetGameProviderCultureCodeAsync(
			Arg.Is(gameProviderId),
			Arg.Is(query.Language),
			Arg.Any<CancellationToken>())
			.Returns(gameProviderCultureCode);

		var gameInfo = new GameInfo(
			GameId: 1,
			GameCode: "test",
			GameProviderId: Guid.NewGuid(),
			GameProviderCode: GameProviderType.JiliTransfer,
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
			SubGameProviderCode: GameProviderType.JiliTransfer);
		_ = fakeGameRepository.GetAsync(
			Arg.Is(query.GameId.Value),
			Arg.Any<CancellationToken>())
			.Returns(gameInfo);

		var response = new GameUrlReply()
		{
			IsPlayerExist = true,
			GameUrl = new Uri("http://www.google.com/").ToString()
		};
		_ = fakeClient.GameUrlAsync(
			apiUrl: Arg.Is(configuration.ApiUrl),
			agentId: Arg.Is(configuration.AgentId),
			agentKey: Arg.Is(configuration.AgentKey),
			playerCode: Arg.Is(playerInfo.PlayerCode),
			externalGameId: Arg.Is(gameInfo.GameExternalId),
			language: Arg.Is(gameProviderCultureCode))
			.Returns(response);

		var sut = new JiliTransferGameUrlGetService(
			fakeBrandService,
			fakeGameRepository,
			fakeGameProviderRepository,
			fakeClient,
			fakePlayerRepository);

		var actual = await sut.GetAsync(query);

		Assert.NotNull(actual);
		Assert.Equal(new Uri(response.GameUrl), actual);
	}
}
