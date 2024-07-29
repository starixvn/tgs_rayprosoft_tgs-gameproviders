using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.MicroGaming.Client;
using Tgs.GameInterface.MicroGaming.Protos;
using Tgs.GameProviders.Core.MicroGaming.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.MicroGaming;

public class MicroGamingPlayerBalanceGetServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeBrandRepository = Substitute.For<IBrandRepository>();
		var fakeClient = Substitute.For<IGameInterfaceMicroGamingClient>();

		var query = new MicroGamingPlayerBalanceQuery
		{
			PlayerCode = "4ucmqplxb029"
		};

		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProviderRepository.GetGameProviderIdAsync(
			gameProviderCode: Arg.Is(GameProviderType.MicroGaming),
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

		var configuration = new MicroGamingConfiguration()
		{
			StsApiUrl = new("https://localhost/sts/"),
			ClientId = "clientId",
			ClientSecret = "clientSecret",
			ApiUrl = new("https://localhost/"),
			AgentCode = "agentCode",
		};
		_ = fakeGameProviderRepository.GetMicroGamingConfigurationAsync(
			gameProviderConfigurationId: Arg.Is(gameProviderConfigurationId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(configuration);

		var balance = new PlayerBalanceReply
		{
			IsPlayerExist = true,
			Balance = 100.0m
		};
		_ = fakeClient.PlayerBalanceAsync(
			stsApiBaseUrl: Arg.Is(configuration.StsApiUrl),
			clientId: Arg.Is(configuration.ClientId),
			clientSecret: Arg.Is(configuration.ClientSecret),
			apiBaseUrl: Arg.Is(configuration.ApiUrl),
			agentCode: Arg.Is(configuration.AgentCode),
			playerCode: Arg.Is(player.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(balance);

		var sut = new MicroGamingPlayerBalanceGetService(
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
		var fakeClient = Substitute.For<IGameInterfaceMicroGamingClient>();

		var query = new MicroGamingPlayerBalanceQuery
		{
			PlayerCode = "4ucmqplxb029"
		};

		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProviderRepository.GetGameProviderIdAsync(
			gameProviderCode: Arg.Is(GameProviderType.MicroGaming),
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

		var configuration = new MicroGamingConfiguration()
		{
			StsApiUrl = new("https://localhost/sts/"),
			ClientId = "clientId",
			ClientSecret = "clientSecret",
			ApiUrl = new("https://localhost/"),
			AgentCode = "agentCode",
		};
		_ = fakeGameProviderRepository.GetMicroGamingConfigurationAsync(
			gameProviderConfigurationId: Arg.Is(gameProviderConfigurationId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(configuration);

		_ = fakeClient.PlayerBalanceAsync(
			stsApiBaseUrl: Arg.Is(configuration.StsApiUrl),
			clientId: Arg.Is(configuration.ClientId),
			clientSecret: Arg.Is(configuration.ClientSecret),
			apiBaseUrl: Arg.Is(configuration.ApiUrl),
			agentCode: Arg.Is(configuration.AgentCode),
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

		var sut = new MicroGamingPlayerBalanceGetService(
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
			stsApiBaseUrl: Arg.Is(configuration.StsApiUrl),
			clientId: Arg.Is(configuration.ClientId),
			clientSecret: Arg.Is(configuration.ClientSecret),
			apiBaseUrl: Arg.Is(configuration.ApiUrl),
			agentCode: Arg.Is(configuration.AgentCode),
			playerCode: Arg.Is(player.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
