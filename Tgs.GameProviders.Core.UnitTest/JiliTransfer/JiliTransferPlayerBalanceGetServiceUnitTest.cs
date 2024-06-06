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

public class JiliTransferPlayerBalanceGetServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeBrandService = Substitute.For<IBrandRepository>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakeClient = Substitute.For<IGameInterfaceJiliClient>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();

		var query = new JiliTransferPlayerBalanceQuery
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

		var response = new PlayerBalanceReply()
		{
			IsPlayerExist = true,
			Balance = 100m
		};
		_ = fakeClient.PlayerBalanceAsync(
			apiUrl: Arg.Is(configuration.ApiUrl),
			agentId: Arg.Is(configuration.AgentId),
			agentKey: Arg.Is(configuration.AgentKey),
			playerCode: Arg.Is(query.PlayerCode),
			Arg.Any<CancellationToken>())
			.Returns(response);

		var sut = new JiliTransferPlayerBalanceGetService(
			fakeBrandService,
			fakeGameProviderRepository,
			fakeClient,
			fakePlayerRepository);

		var actual = await sut.GetAsync(query);

		Assert.NotNull(actual);
		Assert.Equal(response.Balance.ToDecimal(), actual.Balance);
	}
}
