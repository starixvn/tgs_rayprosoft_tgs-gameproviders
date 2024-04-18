using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.Cards365.Client;
using Tgs.GameProviders.Core.Cards365Transfer.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.Cards365Transfer;

public class Cards365TransferGameUrlGetServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeBrandRepository = Substitute.For<IBrandRepository>();
		var fakeClient = Substitute.For<IGameInterfaceCards365Client>();

		var query = new Cards365TransferGameUrlQuery
		{
			GameType = GameType.CardGameLobby,
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
				gameProviderCode: Arg.Is(GameProviderType.Cards365Transfer),
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

		var configuration = new Cards365TransferConfiguration()
		{
			ApiUrl = new("http://localhost"),
			Website = "website",
			Cert = "cert",
			Extension1 = "extension1"
		};
		_ = fakeGameProviderRepository
			.GetCards365TransferConfigurationAsync(
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
				website: Arg.Is(configuration.Website),
				cert: Arg.Is(configuration.Cert),
				extension1: Arg.Is(configuration.Extension1),
				playerCode: Arg.Is(player.PlayerCode),
				currency: int.Parse(currencyCode),
				language: cultureCode,
				Arg.Any<CancellationToken>())
			.Returns(gameUrl);

		var sut = new Cards365TransferGameUrlGetService(
			fakeGameProviderRepository,
			fakePlayerRepository,
			fakeBrandRepository,
			fakeClient);

		var actual = await sut.GetAsync(query);

		Assert.NotNull(actual);
		Assert.Equal(new Uri(gameUrl), actual);
	}
}
