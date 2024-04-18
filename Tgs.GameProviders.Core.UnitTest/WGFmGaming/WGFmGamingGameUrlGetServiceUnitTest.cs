using System;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.WGFmGamingService.Client;
using Tgs.GameProviders.Core.WGFmGaming.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.WGFmGaming;

public class WGFmGamingGameUrlGetServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeGameInterfaceWGFmGamingClient = Substitute.For<IGameInterfaceWGFmGamingClient>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeBrandRepository = Substitute.For<IBrandRepository>();

		var query = new WGFmGamingGameUrlQuery
		{
			GameType = GameType.Slot,
			GameId = 1,
			BrandCode = "GPI",
			PlayerCode = "test",
			CurrencyCode = "CNY",
			LoginRecordId = Guid.NewGuid(),
			PlatformType = PlatformType.Desktop,
			Language = "en-US",
			IPAddress = "127.0.0.1",
			BackUrl = new("http://localhost/")
		};

		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProviderRepository.GetGameProviderIdAsync(Arg.Is(GameProviderType.WGFmGaming)).Returns(ValueTask.FromResult(gameProviderId));

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
			DefaultWalletCurrencyCode: "CNY");
		_ = fakePlayerRepository.GetAsync(Arg.Is(query.PlayerCode)).Returns(player);

		var gameProviderConfigurationId = Guid.NewGuid();
		_ = fakeBrandRepository.GetBrandGameProviderConfigurationIdAsync(
			Arg.Is(gameProviderId),
			Arg.Is(player.BrandId)).Returns(ValueTask.FromResult(gameProviderConfigurationId));

		var configuration = new WGFmGamingConfiguration
		{
			ApiUrl = new("http://localhost/"),
			PartnerCode = "PartnerCode",
			PartnerKey = "PartnerKey",
			IV = "IV",
			Version = 2
		};
		_ = fakeGameProviderRepository.GetWGFmGamingConfigurationAsync(
			Arg.Is(gameProviderConfigurationId)
			).Returns(ValueTask.FromResult(configuration));

		var cultureCode = "2";
		_ = fakeGameProviderRepository.GetGameProviderCultureCodeAsync(
			Arg.Is(gameProviderId),
			Arg.Is(query.Language)).Returns(cultureCode);

		var currencyCode = "RMB";
		_ = fakeGameProviderRepository.GetGameProviderCurrencyCodeAsync(
			Arg.Is(gameProviderId),
			Arg.Is(query.CurrencyCode)).Returns(currencyCode);

		var token = Guid.NewGuid().ToString("N");
		_ = fakeGameInterfaceWGFmGamingClient
			 .GamePrepareLoginAsync(
				 Arg.Is(configuration.ApiUrl),
				 Arg.Is(configuration.PartnerCode),
				 Arg.Is(configuration.PartnerKey),
				 Arg.Is(configuration.IV),
				 Arg.Is(player.PlayerCode),
				 Arg.Is(currencyCode))
			 .Returns(token);

		var gameUrl = new Uri("http://localhost/");
		_ = fakeGameInterfaceWGFmGamingClient
			.GameLoginAsync(
				Arg.Is(configuration.ApiUrl),
				Arg.Is(configuration.PartnerCode),
				Arg.Is(token),
				Arg.Is(cultureCode),
				Arg.Is(configuration.Version))
			.Returns(gameUrl);

		var sut = new WGFmGamingGameUrlGetService(
			fakeGameInterfaceWGFmGamingClient,
			fakeGameProviderRepository,
			fakePlayerRepository,
			fakeBrandRepository);

		var actual = await sut.GetAsync(query);

		Assert.NotNull(actual);
		Assert.Equal(gameUrl, actual);
	}

	[Fact]
	public async Task Normal_Process_Version_1()
	{
		var fakeGameInterfaceWGFmGamingClient = Substitute.For<IGameInterfaceWGFmGamingClient>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeBrandRepository = Substitute.For<IBrandRepository>();

		var query = new WGFmGamingGameUrlQuery
		{
			GameType = GameType.Slot,
			GameId = 1,
			BrandCode = "GPI",
			PlayerCode = "test",
			CurrencyCode = "CNY",
			LoginRecordId = Guid.NewGuid(),
			PlatformType = PlatformType.Desktop,
			Language = "en-US",
			IPAddress = "127.0.0.1",
			BackUrl = new("http://localhost/")
		};

		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProviderRepository.GetGameProviderIdAsync(Arg.Is(GameProviderType.WGFmGaming)).Returns(ValueTask.FromResult(gameProviderId));

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
			DefaultWalletCurrencyCode: "CNY");
		_ = fakePlayerRepository.GetAsync(Arg.Is(query.PlayerCode)).Returns(player);

		var gameProviderConfigurationId = Guid.NewGuid();
		_ = fakeBrandRepository.GetBrandGameProviderConfigurationIdAsync(
			Arg.Is(gameProviderId),
			Arg.Is(player.BrandId)).Returns(ValueTask.FromResult(gameProviderConfigurationId));

		var configuration = new WGFmGamingConfiguration
		{
			ApiUrl = new("http://localhost/"),
			PartnerCode = "PartnerCode",
			PartnerKey = "PartnerKey",
			IV = "IV",
			Version = 1
		};
		_ = fakeGameProviderRepository.GetWGFmGamingConfigurationAsync(
			Arg.Is(gameProviderConfigurationId)
			).Returns(ValueTask.FromResult(configuration));

		var cultureCode = "2";
		_ = fakeGameProviderRepository.GetGameProviderCultureCodeAsync(
			Arg.Is(gameProviderId),
			Arg.Is(query.Language)).Returns(cultureCode);

		var currencyCode = "RMB";
		_ = fakeGameProviderRepository.GetGameProviderCurrencyCodeAsync(
			Arg.Is(gameProviderId),
			Arg.Is(query.CurrencyCode)).Returns(currencyCode);

		var token = Guid.NewGuid().ToString("N");
		_ = fakeGameInterfaceWGFmGamingClient
			 .GamePrepareLoginAsync(
				 Arg.Is(configuration.ApiUrl),
				 Arg.Is(configuration.PartnerCode),
				 Arg.Is(configuration.PartnerKey),
				 Arg.Is(configuration.IV),
				 Arg.Is(player.PlayerCode),
				 Arg.Is(currencyCode))
			 .Returns(token);

		var gameUrl = new Uri("http://localhost/");
		_ = fakeGameInterfaceWGFmGamingClient
			.GameLoginAsync(
				Arg.Is(configuration.ApiUrl),
				Arg.Is(configuration.PartnerCode),
				Arg.Is(token),
				Arg.Is(cultureCode),
				Arg.Is(configuration.Version))
			.Returns(gameUrl);

		var sut = new WGFmGamingGameUrlGetService(
			fakeGameInterfaceWGFmGamingClient,
			fakeGameProviderRepository,
			fakePlayerRepository,
			fakeBrandRepository);

		var actual = await sut.GetAsync(query);

		Assert.NotNull(actual);
		Assert.Equal(gameUrl, actual);
	}
}
