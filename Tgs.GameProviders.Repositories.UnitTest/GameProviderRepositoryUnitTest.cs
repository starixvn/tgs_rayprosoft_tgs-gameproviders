using System.Text.Json.Nodes;
using NSubstitute;
using Tgs.GameCommand.Client;
using Tgs.GameCommand.Common;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Repositories.UnitTest;

public class GameProviderRepositoryUnitTest
{
	[Fact]
	public async Task Get_Game_Endpoints()
	{
		var fakeClient = Substitute.For<IGameCommandClient>();

		var desktopEndpoint = "http://desktop.google.com";
		var mobileEndpoint = "http://mobile.google.com";
		var gameProviderId = Guid.NewGuid();
		var gameProviderConfigurationId = Guid.NewGuid();

		_ = fakeClient.GameProviderConfigurationGetAsync(gameProviderConfigurationId).Returns(new GameProviderConfigInfo(
			GameProviderId: gameProviderId,
			GameProviderCode: "AG",
			GameProviderConfigId: gameProviderConfigurationId,
			Name: "AG-PG",
			Description: "AG-PG Configuration",
			ClientId: "AG-PG",
			SecretKey: Guid.NewGuid().ToString("N"),
			Status: GameProviderConfigStatus.Active,
			DesktopEndpoint: desktopEndpoint,
			MobileEndpoint: mobileEndpoint,
			ConfigurationData: string.Empty,
			IsDefault: false));

		var repository = new GameProviderRepository(fakeClient);

		var (DesktopEndpoint, MobileEndpoint) = await repository.GetGameEndpointsAsync(gameProviderConfigurationId);

		Assert.Equal(desktopEndpoint, DesktopEndpoint);
		Assert.Equal(mobileEndpoint, MobileEndpoint);
	}

	[Fact]
	public async Task Get_GameProvider()
	{
		var fakeClient = Substitute.For<IGameCommandClient>();

		var gameProviderId = Guid.NewGuid();

		_ = fakeClient.GameProviderGetAsync(gameProviderId).Returns(new GameProviderInfo(
			GameProviderId: gameProviderId,
			Description: "KY",
			AutoCloseRound: true,
			SupportTestPlayer: true,
			AllowKickPlayer: true,
			PlaceBetTimeout: 0,
			SettleBetTimeout: 0,
			UniqueId: 1,
			TransferWallet: true,
			GameProviderCode: "KY",
			Name: "KY",
			Localization: null));

		var repository = new GameProviderRepository(fakeClient);

		var actual = await repository.GetAsync(gameProviderId);

		Assert.NotNull(actual);
		Assert.Equal(gameProviderId, actual.GameProviderId);
	}

	[Fact]
	public async Task Get_GameProvider_CultureCode()
	{
		var fakeClient = Substitute.For<IGameCommandClient>();

		var gameProviderId = Guid.NewGuid();
		var cultureCode = "en-US";
		var expected = "en";
		_ = fakeClient.GameProviderGetCultureCodeAsync(gameProviderId, cultureCode).Returns(expected);

		var repository = new GameProviderRepository(fakeClient);

		var actual = await repository.GetGameProviderCultureCodeAsync(gameProviderId, cultureCode);

		Assert.NotNull(actual);
		Assert.Equal(expected, actual);
	}

	[Fact]
	public async Task Get_GameProvider_CurrencyCode()
	{
		var fakeClient = Substitute.For<IGameCommandClient>();

		var gameProviderId = Guid.NewGuid();
		var currencyCode = "CNY";
		var expected = "RMB";
		_ = fakeClient.GameProviderGetCurrencyCodeAsync(gameProviderId, currencyCode).Returns(expected);

		var repository = new GameProviderRepository(fakeClient);

		var actual = await repository.GetGameProviderCurrencyCodeAsync(gameProviderId, currencyCode);

		Assert.NotNull(actual);
		Assert.Equal(expected, actual);
	}

	[Fact]
	public async Task Get_GameProvider_Id()
	{
		var fakeClient = Substitute.For<IGameCommandClient>();

		var gameProviderCode = "KY";
		var expected = Guid.NewGuid();
		_ = fakeClient.GameProviderGetGameProviderIdAsync(gameProviderCode).Returns(expected);

		var repository = new GameProviderRepository(fakeClient);

		var actual = await repository.GetGameProviderIdAsync(gameProviderCode);

		Assert.Equal(expected, actual);
	}
}
