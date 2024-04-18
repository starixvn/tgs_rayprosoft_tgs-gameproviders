using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.WicketCricket.Client;
using Tgs.GameProviders.Core.WicketCricket.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.WicketCricket;

public class WicketCricketGameUrlGetServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeBrandRepository = Substitute.For<IBrandRepository>();
		var fakeClient = Substitute.For<IGameInterfaceWicketCricketClient>();

		var query = new WicketCricketGameUrlQuery
		{
			GameType = GameType.Sportsbook,
			GameId = 1,
			BrandCode = "GPI",
			PlayerCode = "4ucmqplxb029",
			CurrencyCode = "CNY",
			LoginRecordId = Guid.NewGuid(),
			PlatformType = PlatformType.Desktop,
			Language = "zh-CN",
			IPAddress = "127.0.0.1",
			BackUrl = new("https://dev-t1.rpgp.cc/")
		};

		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProviderRepository
			.GetGameProviderIdAsync(
			gameProviderCode: Arg.Is(GameProviderType.WicketCricket),
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

		var configuration = new WicketCricketConfiguration()
		{
			ApiUrl = new("http://localhost"),
			ExchangeApiUrl = new("http://localhost"),
			Website = "Website",
			Cert = "Cert",
			Agent = "Agent"
		};
		_ = fakeGameProviderRepository
			.GetWicketCricketConfigurationAsync(
			gameProviderConfigurationId: Arg.Is(gameProviderConfigurationId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(configuration);

		var currencyCode = "2";
		_ = fakeGameProviderRepository
			.GetGameProviderCurrencyCodeAsync(
				gameProviderId: gameProviderId,
				currencyCode: player.DefaultWalletCurrencyCode,
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(currencyCode);

		var gameUrl = "http://localhost:8088/swagger";

		_ = fakeClient
			.GameUrlAsync(
			apiUrl: Arg.Is(configuration.ApiUrl),
			exchangeApiUrl: Arg.Is(configuration.ExchangeApiUrl),
			website: Arg.Is(configuration.Website),
			cert: Arg.Is(configuration.Cert),
			agent: Arg.Is(configuration.Agent),
			playerCode: Arg.Is(player.PlayerCode),
			currency: Arg.Is(int.Parse(currencyCode)),
			Arg.Any<CancellationToken>())
			.Returns(gameUrl);

		var sut = new WicketCricketGameUrlGetService(
			fakeGameProviderRepository,
			fakePlayerRepository,
			fakeBrandRepository,
			fakeClient);

		var actual = await sut.GetAsync(query);

		Assert.NotNull(actual);
		Assert.Equal(gameUrl, actual);
	}
}
