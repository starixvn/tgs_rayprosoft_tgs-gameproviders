using System;
using System.Text.Json.Nodes;

using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.Ga28.Client;
using Tgs.GameInterface.Ga28.Protos;
using Tgs.GameProviders.Core.GA28.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.GA28;

public class GA28PlayerFundInGetServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeBrandRepository = Substitute.For<IBrandRepository>();
		var fakeClient = Substitute.For<IGameInterfaceGa28Client>();

		var query = new GA28PlayerFundInQuery
		{
			PlayerCode = "4ucmqplxb029",
			OrderId = "01J0A7QY0ZRM8KDYE4BBZPZPER",
			Amount = 100
		};

		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProviderRepository.GetGameProviderIdAsync(
			gameProviderCode: Arg.Is(GameProviderType.GA28),
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

		var configuration = new GA28Configuration()
		{
			ApiUrl = new("https://localhost/"),
			PrivateKey = "privateKey",
			PartnerAccountId = "partnerAccountId"
		};
		_ = fakeGameProviderRepository.GetGA28ConfigurationAsync(
			gameProviderConfigurationId: Arg.Is(gameProviderConfigurationId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(configuration);

		var reply = new FundReply
		{
			OrderId = "01J0A7QY0ZRM8KDYE4BBZPZPER",
			ExternalOrderId = "01J0A86KKR5458KM7Y7WGM7X69",
			Balance = 0.0m,
			Resp = new JsonObject { }.ToJsonString()
		};
		_ = fakeClient.FundInAsync(
			apiUrl: Arg.Is(configuration.ApiUrl),
			privateKey: Arg.Is(configuration.PrivateKey),
			playerCode: Arg.Is(player.PlayerCode),
			orderId: Arg.Is(query.OrderId),
			amount: Arg.Is(query.Amount),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(reply);

		var sut = new GA28PlayerFundInGetService(
			fakeGameProviderRepository,
			fakePlayerRepository,
			fakeBrandRepository,
			fakeClient);

		var actual = await sut.GetAsync(query);

		Assert.NotNull(actual);
		Assert.Equal(query.OrderId, actual.OrderId);
		Assert.Equal("01J0A86KKR5458KM7Y7WGM7X69", actual.ExternalOrderId);
	}
}
