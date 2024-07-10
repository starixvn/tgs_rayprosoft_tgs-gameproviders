using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.GlobalTechnology.Client;
using Tgs.GameInterface.GlobalTechnology.Protos;
using Tgs.GameProviders.Core.GTPlaytech.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.GTPlaytech;

public class GTPlaytechPlayerBalanceGetServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeBrandRepository = Substitute.For<IBrandRepository>();
		var fakeClient = Substitute.For<IGameInterfaceGlobalTechnologyClient>();

		var query = new GTPlaytechPlayerBalanceQuery
		{
			PlayerCode = "4ucmqplxb029"
		};

		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProviderRepository.GetGameProviderIdAsync(
			gameProviderCode: Arg.Is(GameProviderType.GT_Playtech),
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

		var configuration = new GTPlaytechConfiguration()
		{
			ApiUrl = new("https://api-stg.gmgiantgold.com/"),
			LauncherApiUrl = new("https://api-stg.gmgiantgold.com/"),
			DataApiUrl = new("http://gm-stage-data.gmgiantgold.com/"),
			BrandId = 911,
			SecretKey = "pPRHm1NHFosSOUv4tVjv"
		};
		_ = fakeGameProviderRepository.GetGTPlaytechConfigurationAsync(
			gameProviderConfigurationId: Arg.Is(gameProviderConfigurationId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(configuration);

		var currency = "CNY";
		_ = fakeGameProviderRepository.GetGameProviderCurrencyCodeAsync(
			gameProviderId: Arg.Is(gameProviderId),
			currencyCode: Arg.Is(player.DefaultWalletCurrencyCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(currency);

		var balance = new PlayerBalanceReply
		{
			IsPlayerExist = true,
			Balance = 100.0m
		};
		_ = fakeClient.PlayerBalanceAsync(
			apiUrl: Arg.Is(configuration.ApiUrl),
			brandId: Arg.Is(configuration.BrandId),
			secretKey: Arg.Is(configuration.SecretKey),
			playerCode: Arg.Is(player.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(balance);

		var sut = new GTPlaytechPlayerBalanceGetService(
			fakeGameProviderRepository,
			fakePlayerRepository,
			fakeBrandRepository,
			fakeClient);

		var actual = await sut.GetAsync(query);

		Assert.NotNull(actual);
		Assert.Equal(100.0m, actual.Balance);

		_ = fakeClient
			.Received(0)
			.PlayerCreateAsync(
			apiUrl: Arg.Is(configuration.ApiUrl),
			brandId: Arg.Is(configuration.BrandId),
			secretKey: Arg.Is(configuration.SecretKey),
			playerCode: Arg.Is(player.PlayerCode),
			currency: Arg.Is(currency),
			language: Arg.Is("EN"),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task Player_does_not_exist()
	{
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeBrandRepository = Substitute.For<IBrandRepository>();
		var fakeClient = Substitute.For<IGameInterfaceGlobalTechnologyClient>();

		var query = new GTPlaytechPlayerBalanceQuery
		{
			PlayerCode = "4ucmqplxb029"
		};

		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProviderRepository.GetGameProviderIdAsync(
			gameProviderCode: Arg.Is(GameProviderType.GT_Playtech),
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

		var configuration = new GTPlaytechConfiguration()
		{
			ApiUrl = new("https://api-stg.gmgiantgold.com/"),
			LauncherApiUrl = new("https://api-stg.gmgiantgold.com/"),
			DataApiUrl = new("http://gm-stage-data.gmgiantgold.com/"),
			BrandId = 911,
			SecretKey = "pPRHm1NHFosSOUv4tVjv"
		};
		_ = fakeGameProviderRepository.GetGTPlaytechConfigurationAsync(
			gameProviderConfigurationId: Arg.Is(gameProviderConfigurationId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(configuration);

		var currency = "CNY";
		_ = fakeGameProviderRepository.GetGameProviderCurrencyCodeAsync(
			gameProviderId: Arg.Is(gameProviderId),
			currencyCode: Arg.Is(player.DefaultWalletCurrencyCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(currency);

		_ = fakeClient.PlayerBalanceAsync(
			apiUrl: Arg.Is(configuration.ApiUrl),
			brandId: Arg.Is(configuration.BrandId),
			secretKey: Arg.Is(configuration.SecretKey),
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

		var sut = new GTPlaytechPlayerBalanceGetService(
			fakeGameProviderRepository,
			fakePlayerRepository,
			fakeBrandRepository,
			fakeClient);

		var actual = await sut.GetAsync(query);

		Assert.NotNull(actual);
		Assert.Equal(100.0m, actual.Balance);

		_ = fakeClient
			.Received(1)
			.PlayerCreateAsync(
			apiUrl: Arg.Is(configuration.ApiUrl),
			brandId: Arg.Is(configuration.BrandId),
			secretKey: Arg.Is(configuration.SecretKey),
			playerCode: Arg.Is(player.PlayerCode),
			currency: Arg.Is(currency),
			language: Arg.Is("EN"),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
