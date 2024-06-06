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

namespace Tgs.GameProviders.Core.UnitTest.JiliTransfer;

public class JiliTransferPlayerOnlineGetServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeBrandService = Substitute.For<IBrandRepository>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakeClient = Substitute.For<IGameInterfaceJiliClient>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();

		var query = new JiliTransferPlayerOnlineQuery
		{
			PlayerCode = "4ucmqplxb029"
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

		_ = fakeClient.PlayerOnlineAsync(
			apiUrl: Arg.Is(configuration.ApiUrl),
			agentId: Arg.Is(configuration.AgentId),
			agentKey: Arg.Is(configuration.AgentKey),
			playerCode: Arg.Is(query.PlayerCode),
			Arg.Any<CancellationToken>())
			.Returns(true);

		var sut = new JiliTransferPlayerOnlineGetService(
			fakeGameProviderRepository,
			fakePlayerRepository,
			fakeBrandService,
			fakeClient);

		var actual = await sut.GetAsync(query);

		Assert.True(actual);
	}
}
