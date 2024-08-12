using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.Pinnacle.Client;
using Tgs.GameProviders.Core.Pinnacle.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.Pinnacle;

public class PinnacleGameUrlGetServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeBrandRepository = Substitute.For<IBrandRepository>();
		var fakeClient = Substitute.For<IGameInterfacePinnacleClient>();

		var query = new PinnacleGameUrlQuery
		{
			PlayerCode = "4ucmqplxb029",
			GameId = 1,
			LoginRecordId = Guid.NewGuid().ToString(),
			Language = "zh-CN",
			PlatformType = PlatformType.Desktop,
			IPAddress = "127.0.0.1",
			BackUrl = new("https://dev-t1.rpgp.cc/")
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

		var currencyCode = "3";
		_ = fakeGameProviderRepository
			.GetGameProviderCurrencyCodeAsync(
				gameProviderId: gameProviderId,
				currencyCode: player.DefaultWalletCurrencyCode,
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(currencyCode);

		var cultureCode = "en";
		_ = fakeGameProviderRepository
			.GetGameProviderCultureCodeAsync(
				gameProviderId: Arg.Is(gameProviderId),
				cultureCode: Arg.Is(query.Language),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(cultureCode);

		var gameUrl = "http://localhost:8088/swagger";

		_ = fakeClient
			.GameUrlAsync(
				apiUrl: Arg.Is(configuration.ApiUrl),
				agentCode: Arg.Is(configuration.AgentCode),
				agentKey: Arg.Is(configuration.AgentKey),
				secretKey: Arg.Is(configuration.SecretKey),
				playerCode: Arg.Is(player.PlayerCode),
				locale: cultureCode,
				Arg.Any<CancellationToken>())
			.Returns(gameUrl);

		var sut = new PinnacleGameUrlGetService(
			fakeGameProviderRepository,
			fakePlayerRepository,
			fakeBrandRepository,
			fakeClient);

		var actual = await sut.GetAsync(query);

		Assert.NotNull(actual);
		Assert.Equal(gameUrl, actual);
	}
}
