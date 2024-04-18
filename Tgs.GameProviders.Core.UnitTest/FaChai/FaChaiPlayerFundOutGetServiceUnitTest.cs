using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.FaChai.Client;
using Tgs.GameInterface.FaChai.Protos;
using Tgs.GameProviders.Core.FaChai.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.FaChai;

public class FaChaiPlayerFundOutGetServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeBrandRepository = Substitute.For<IBrandRepository>();
		var fakeSnowflakeWork = Substitute.For<ISnowflakeWork>();
		var fakeClient = Substitute.For<IGameInterfaceFaChaiClient>();

		var query = new FaChaiPlayerFundOutQuery
		{
			BrandCode = "GPI",
			PlayerCode = "4ucmqplxb029",
			CurrencyCode = "CNY",
			Amount = 100M,
			OrderId = Guid.NewGuid().ToString(),
		};

		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProviderRepository
			.GetGameProviderIdAsync(
				gameProviderCode: Arg.Is(GameProviderType.FaChai),
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

		var configuration = new FaChaiConfiguration()
		{
			ApiUrl = new("https://api.dg0.co/"),
			AgentCode = "GPICN",
			AgentKey = "EOJC9q31Y7AnPb9i",
			Currency = "CNY"
		};
		_ = fakeGameProviderRepository
			.GetFaChaiConfigurationAsync(
				gameProviderConfigurationId: Arg.Is(gameProviderConfigurationId),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(configuration);

		var orderId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
		_ = fakeSnowflakeWork.GetId().Returns(orderId);

		var response = new FundReply()
		{
			TrsId = orderId.ToString(),
			BankId = 1635165954039484416,
			Balance = 0m,
			Amount = -100m
		};

		_ = fakeClient.FundOutAsync(
			apiUrl: Arg.Is(configuration.ApiUrl),
			agentCode: Arg.Is(configuration.AgentCode),
			agentKey: Arg.Is(configuration.AgentKey),
			currency: Arg.Is(configuration.Currency),
			playerCode: Arg.Is(player.PlayerCode),
			orderId: Arg.Is(orderId.ToString()),
			amount: Arg.Is(query.Amount),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(response);

		var sut = new FaChaiPlayerFundOutGetService(
			fakeGameProviderRepository,
			fakePlayerRepository,
			fakeBrandRepository,
			fakeSnowflakeWork,
			fakeClient);

		var actual = await sut.GetAsync(query);

		Assert.NotNull(actual);
		Assert.Equal(response.TrsId, actual.OrderId);
	}
}
