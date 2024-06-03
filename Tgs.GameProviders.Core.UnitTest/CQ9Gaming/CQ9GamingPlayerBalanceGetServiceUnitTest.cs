using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.CQ9Gaming.Client;
using Tgs.GameInterface.CQ9Gaming.Protos;
using Tgs.GameProviders.Core.CQ9Gaming.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.CQ9Gaming;

public class CQ9GamingPlayerBalanceGetServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeBrandRepository = Substitute.For<IBrandRepository>();
		var fakeClient = Substitute.For<IGameInterfaceCQ9GamingClient>();

		var query = new CQ9GamingPlayerBalanceQuery
		{
			PlayerCode = "4ucmqplxb029"
		};

		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProviderRepository.GetGameProviderIdAsync(
			gameProviderCode: Arg.Is(GameProviderType.CQ9Gaming),
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

		var configuration = new CQ9GamingConfiguration()
		{
			ApiUrl = new("https://localhost/"),
			ApiToken = "apiToken",
		};
		_ = fakeGameProviderRepository.GetCQ9GamingConfigurationAsync(
			gameProviderConfigurationId: Arg.Is(gameProviderConfigurationId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(configuration);

		var balance = new PlayerBalanceReply
		{
			IsPlayerExist = true,
			Balance = 100.0m
		};
		_ = fakeClient.PlayerBalanceAsync(
			apiUrl: Arg.Is(configuration.ApiUrl),
			apiToken: Arg.Is(configuration.ApiToken),
			playerCode: Arg.Is(player.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(balance);

		var sut = new CQ9GamingPlayerBalanceGetService(
			fakeGameProviderRepository,
			fakePlayerRepository,
			fakeBrandRepository,
			fakeClient);

		var actual = await sut.GetAsync(query);

		Assert.NotNull(actual);
		Assert.Equal(100.0m, actual.Balance);
	}

	[Fact]
	public async Task Player_does_not_exist()
	{
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeBrandRepository = Substitute.For<IBrandRepository>();
		var fakeClient = Substitute.For<IGameInterfaceCQ9GamingClient>();

		var query = new CQ9GamingPlayerBalanceQuery
		{
			PlayerCode = "4ucmqplxb029"
		};

		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProviderRepository.GetGameProviderIdAsync(
			gameProviderCode: Arg.Is(GameProviderType.CQ9Gaming),
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

		var configuration = new CQ9GamingConfiguration()
		{
			ApiUrl = new("https://localhost/"),
			ApiToken = "apiToken",
		};
		_ = fakeGameProviderRepository.GetCQ9GamingConfigurationAsync(
			gameProviderConfigurationId: Arg.Is(gameProviderConfigurationId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(configuration);

		_ = fakeClient.PlayerBalanceAsync(
			apiUrl: Arg.Is(configuration.ApiUrl),
			apiToken: Arg.Is(configuration.ApiToken),
			playerCode: Arg.Is(player.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(
			  new PlayerBalanceReply
			  {
				  IsPlayerExist = false,
				  Balance = 0.0m
			  },
			   new PlayerBalanceReply
			   {
				   IsPlayerExist = true,
				   Balance = 100.0m
			   });

		var sut = new CQ9GamingPlayerBalanceGetService(
			fakeGameProviderRepository,
			fakePlayerRepository,
			fakeBrandRepository,
			fakeClient);

		var actual = await sut.GetAsync(query);

		Assert.NotNull(actual);
		Assert.Equal(100.0m, actual.Balance);

		await fakeClient
			.Received(1)
			.PlayerCreateAsync(
			apiUrl: Arg.Is(configuration.ApiUrl),
			apiToken: Arg.Is(configuration.ApiToken),
			playerCode: Arg.Is(player.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
