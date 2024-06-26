using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.V8Poker.Client;
using Tgs.GameProviders.Core.V8Poker.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.V8Poker;

public class V8PokerPlayerOnlineGetServiceUnitTest
{
	[Fact]
	public async Task Normal_ProcessAsync()
	{
		var fakeClient = Substitute.For<IGameInterfaceV8PokerClient>();
		var fakeGameProvider = Substitute.For<IGameProviderRepository>();
		var fakePlayer = Substitute.For<IPlayerRepository>();
		var fakeBrand = Substitute.For<IBrandRepository>();

		var query = new V8PokerPlayerOnlineQuery
		{
			PlayerCode = "test"
		};

		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProvider.GetGameProviderIdAsync(
			gameProviderCode: Arg.Is(GameProviderType.V8Poker),
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

		var configuration = new V8PokerConfiguration
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
			.GetV8PokerConfigurationAsync(
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

		var sut = new V8PokerPlayerOnlineGetService(
			fakeGameProvider,
			fakePlayer,
			fakeBrand,
			fakeClient);

		var actual = await sut.GetAsync(query);

		Assert.True(actual);
	}
}
