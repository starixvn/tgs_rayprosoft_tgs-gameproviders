using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.Pinnacle.Client;
using Tgs.GameInterface.Pinnacle.Protos;
using Tgs.GameProviders.Core.Pinnacle.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.Pinnacle;

public class PinnaclePlayerFundInGetServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeBrandRepository = Substitute.For<IBrandRepository>();
		var fakeClient = Substitute.For<IGameInterfacePinnacleClient>();

		var query = new PinnaclePlayerFundInQuery
		{
			PlayerCode = "4ucmqplxb029",
			Amount = 100M,
			OrderId = Guid.Parse("7f5a44a8-74bf-4927-8152-b9482a42af9c").ToString("N"),
		};

		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProviderRepository
			.GetGameProviderIdAsync(
				gameProviderCode: Arg.Is(GameProviderType.Pinnacle),
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

		var configuration = new PinnacleConfiguration()
		{
			ApiUrl = new("http://localhost"),
			AgentCode = "V0201",
			AgentKey = "8d0e9ac0-99d8-48c7-8307-bc328c4181f6",
			SecretKey = "zenDIdY7Knuzmeen"
		};
		_ = fakeGameProviderRepository
			.GetPinnacleConfigurationAsync(
				gameProviderConfigurationId: Arg.Is(gameProviderConfigurationId),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(configuration);

		var response = new FundReply()
		{
			Resp = "{\"amount\":1,\"loginId\":\"4ucmqplxb029\",\"userCode\":\"V020100001\",\"availableBalance\":1.00}"
		};

		_ = fakeClient
			.FundInAsync(
				apiUrl: Arg.Is(configuration.ApiUrl),
				agentCode: Arg.Is(configuration.AgentCode),
				agentKey: Arg.Is(configuration.AgentKey),
				secretKey: Arg.Is(configuration.SecretKey),
				playerCode: Arg.Is(player.PlayerCode),
				transactionId: Arg.Is(query.OrderId),
				amount: Arg.Is(query.Amount),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(response);

		var sut = new PinnaclePlayerFundInGetService(
			fakeGameProviderRepository,
			fakePlayerRepository,
			fakeBrandRepository,
			fakeClient);

		var actual = await sut.GetAsync(query);

		Assert.NotNull(actual);
		Assert.Equal(query.OrderId, actual.OrderId);
		Assert.Empty(actual.ExternalOrderId);
	}
}
