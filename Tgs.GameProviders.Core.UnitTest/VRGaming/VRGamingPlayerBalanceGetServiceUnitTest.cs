using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.VRGaming.Client;
using Tgs.GameInterface.VRGaming.Protos;
using Tgs.GameProviders.Core.VRGaming.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.VRGaming;

public class VRGamingPlayerBalanceGetServiceUnitTest
{
	[Fact]
	public async Task Normal_ProcessAsync()
	{
		var fakeClient = Substitute.For<IGameInterfaceVRGamingClient>();
		var fakeGameProvider = Substitute.For<IGameProviderRepository>();
		var fakePlayer = Substitute.For<IPlayerRepository>();
		var fakeBrand = Substitute.For<IBrandRepository>();

		var query = new VRGamingPlayerBalanceQuery
		{
			PlayerCode = "test"
		};

		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProvider.GetGameProviderIdAsync(
			gameProviderCode: Arg.Is(GameProviderType.VRGaming),
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

		var configuration = new VRGamingConfiguration
		{
			AmountMaximum = 3000000m,
			AmountFormat = "10,3",
			ApiUrl = new("http://localhost/"),
			Version = "1.0",
			MerchantId = "merchantId",
			Deskey = "deskey"
		};
		_ = fakeGameProvider
			.GetVRGamingConfigurationAsync(
			gameProviderConfigurationId: Arg.Is(gameProviderConfigurationId),
			cancellationToken: Arg.Any<CancellationToken>()).
			Returns(ValueTask.FromResult(configuration));

		var playerBalanceReply = new PlayerBalanceReply
		{
			IsPlayerExist = true,
			Balance = 100
		};
		_ = fakeClient
			.PlayerBalanceAsync(
			apiUrl: Arg.Is(configuration.ApiUrl),
			version: Arg.Is(configuration.Version),
			merchantId: Arg.Is(configuration.MerchantId),
			deskey: Arg.Is(configuration.Deskey),
			playerCode: Arg.Is(player.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(playerBalanceReply);

		var sut = new VRGamingPlayerBalanceGetService(
			fakeGameProvider,
			fakePlayer,
			fakeBrand,
			fakeClient);

		var actual = await sut.GetAsync(query);

		Assert.Equal(100m, actual.Balance);
		Assert.Equal(100m, actual.FreeBalance);
		Assert.Equal(0m, actual.OutstandingBalance);
	}
}
