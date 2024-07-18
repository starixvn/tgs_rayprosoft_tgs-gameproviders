using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ReceivedExtensions;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.WicketCricket.Client;
using Tgs.GameProviders.Core.WicketCricket.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.WicketCricket;

public class WicketCricketPlayerLogoutCommandServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeBrandRepository = Substitute.For<IBrandRepository>();
		var fakeClient = Substitute.For<IGameInterfaceWicketCricketClient>();

		var command = new WicketCricketPlayerLogoutCommand
		{
			PlayerCode = "4ucmqplxb029"
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
			playerCode: Arg.Is(command.PlayerCode),
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

		var sut = new WicketCricketPlayerLogoutCommandService(
			fakeGameProviderRepository,
			fakePlayerRepository,
			fakeBrandRepository,
			fakeClient);

		await sut.ExecuteAsync(command);

		_ = fakeClient
			.Received(1)
			.PlayerLogoutAsync(
			apiUrl: Arg.Is(configuration.ApiUrl),
			exchangeApiUrl: Arg.Is(configuration.ExchangeApiUrl),
			website: Arg.Is(configuration.Website),
			cert: Arg.Is(configuration.Cert),
			playerCode: Arg.Is(player.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
