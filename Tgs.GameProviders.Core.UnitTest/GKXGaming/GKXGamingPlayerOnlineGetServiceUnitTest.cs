using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.GKXGaming.Client;
using Tgs.GameProviders.Core.GKXGaming.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.GKXGaming;

public class GKXGamingPlayerOnlineGetServiceUnitTest
{
	[Fact]
	public async Task Normal_ProcessAsync()
	{
		var fakeClient = Substitute.For<IGameInterfaceGKXGamingClient>();
		var fakeGameProvider = Substitute.For<IGameProviderRepository>();
		var fakePlayer = Substitute.For<IPlayerRepository>();
		var fakeBrand = Substitute.For<IBrandRepository>();

		var query = new GKXGamingPlayerOnlineQuery
		{
			PlayerCode = "test"
		};

		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProvider.GetGameProviderIdAsync(
			gameProviderCode: Arg.Is(GameProviderType.GKXGaming),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(ValueTask.FromResult(gameProviderId));

		var player = new PlayerInfo(
			ResellerId: Guid.NewGuid(),
			ResellerCode: "GPI",
			LicenseeId: Guid.NewGuid(),
			LicenseeCode: "GPI",
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			PlayerCode: "test",
			Username: "test",
			DefaultWalletId: Guid.NewGuid(),
			DefaultWalletCurrencyCode: "HKD");
		_ = fakePlayer
			.GetAsync(
			Arg.Is(query.PlayerCode),
			Arg.Any<CancellationToken>())
			.Returns(player);

		var gameProviderConfigurationId = Guid.NewGuid();
		_ = fakeBrand
			.GetBrandGameProviderConfigurationIdAsync(
			gameProviderId: Arg.Is(gameProviderId),
			brandId: Arg.Is(player.BrandId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(ValueTask.FromResult(gameProviderConfigurationId));

		var configuration = new GKXGamingConfiguration
		{
			AmountMaximum = 999999999999,
			AmountFormat = "12,2",
			ApiUrl = new("http://localhost/"),
			ApiRecordUrl = new("http://localhost:8090/"),
			Deskey = "deskey",
			Md5key = "md5key",
			AgentId = "200995"
		};
		_ = fakeGameProvider
			.GetGKXGamingConfigurationAsync(
			gameProviderConfigurationId: Arg.Is(gameProviderConfigurationId),
			cancellationToken: Arg.Any<CancellationToken>()).
			Returns(ValueTask.FromResult(configuration));

		_ = fakeClient.PlayerOnlineAsync(
			apiUrl: Arg.Is(configuration.ApiUrl),
			deskey: Arg.Is(configuration.Deskey),
			md5key: Arg.Is(configuration.Md5key),
			agentId: Arg.Is(configuration.AgentId),
			playerCode: Arg.Is(player.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(true);

		var sut = new GKXGamingPlayerOnlineGetService(
			fakeGameProvider,
			fakePlayer,
			fakeBrand,
			fakeClient);

		var actual = await sut.GetAsync(query);

		Assert.True(actual);
	}
}
