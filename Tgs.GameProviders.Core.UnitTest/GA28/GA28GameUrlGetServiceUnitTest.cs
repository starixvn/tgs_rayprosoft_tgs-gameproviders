using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.Ga28.Client;
using Tgs.GameInterface.Ga28.Protos;
using Tgs.GameProviders.Core.GA28.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.GA28;

public class GA28GameUrlGetServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeBrandRepository = Substitute.For<IBrandRepository>();
		var fakeClient = Substitute.For<IGameInterfaceGa28Client>();

		var query = new GA28GameUrlQuery
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
		_ = fakeGameProviderRepository.GetGameProviderIdAsync(
			gameProviderCode: Arg.Is(GameProviderType.GA28),
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

		var configuration = new GA28Configuration()
		{
			ApiUrl = new("https://localhost/"),
			PrivateKey = "privateKey",
			PartnerAccountId = "partnerAccountId"
		};
		_ = fakeGameProviderRepository.GetGA28ConfigurationAsync(
			gameProviderConfigurationId: Arg.Is(gameProviderConfigurationId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(configuration);

		var cultureCode = "en-us";
		_ = fakeGameProviderRepository.GetGameProviderCultureCodeAsync(
			gameProviderId: Arg.Is(gameProviderId),
			cultureCode: Arg.Is(query.Language),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(cultureCode);

		var gameLoginReply = new GameUrlReply
		{
			GameUrl = "http://localhost/"
		};

		_ = fakeClient.GameUrlAsync(
			apiUrl: Arg.Is(configuration.ApiUrl),
			privateKey: Arg.Is(configuration.PrivateKey),
			partnerAccountId: Arg.Is(configuration.PartnerAccountId),
			playerCode: Arg.Is(player.PlayerCode),
			language: Arg.Is(cultureCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameLoginReply);

		var sut = new GA28GameUrlGetService(
			fakeGameProviderRepository,
			fakePlayerRepository,
			fakeBrandRepository,
			fakeClient);

		var actual = await sut.GetAsync(query);

		Assert.NotNull(actual);
		Assert.NotEmpty(actual.ToString());
	}
}
