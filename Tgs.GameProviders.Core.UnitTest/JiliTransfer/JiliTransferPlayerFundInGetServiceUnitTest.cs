using System;
using System.Text.Json;
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

public class JiliTransferPlayerFundInGetServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeBrandService = Substitute.For<IBrandRepository>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakeClient = Substitute.For<IGameInterfaceJiliClient>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();

		var query = new JiliTransferPlayerFundInQuery
		{
			PlayerCode = "4ucmqplxb029",
			OrderId = Guid.NewGuid().ToString("N"),
			Amount = 100m
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

		var reply = new FundReply()
		{
			TransactionId = query.OrderId,
			CoinAfter = 100,
			CoinBefore = 0,
			CurrencyAfter = 100,
			CurrencyBefore = 0,
			Resp = JsonSerializer.Serialize(new
			{
				TransactionId = query.OrderId,
				CoinAfter = 100,
				CoinBefore = 0,
				CurrencyAfter = 100,
				CurrencyBefore = 0,
			})
		};
		_ = fakeClient.FundInAsync(
			apiUrl: Arg.Is(configuration.ApiUrl),
			agentId: Arg.Is(configuration.AgentId),
			agentKey: Arg.Is(configuration.AgentKey),
			playerCode: Arg.Is(query.PlayerCode),
			transactionId: Arg.Is(query.OrderId),
			amount: Arg.Is(query.Amount),
			Arg.Any<CancellationToken>())
			.Returns(reply);

		var sut = new JiliTransferPlayerFundInGetService(
			fakeBrandService,
			fakeGameProviderRepository,
			fakeClient,
			fakePlayerRepository);

		var actual = await sut.GetAsync(query);

		Assert.NotNull(actual);
		Assert.Equal(query.OrderId, actual.OrderId);
		Assert.Equal(reply.TransactionId, actual.ExternalOrderId);
	}
}
