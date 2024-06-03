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

public class Cards365TransferPlayerFundCheckingGetServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeBrandRepository = Substitute.For<IBrandRepository>();
		var fakeClient = Substitute.For<IGameInterfaceCards365Client>();

		var query = new Cards365TransferPlayerFundCheckingQuery
		{
			PlayerCode = "4ucmqplxb029",
			OrderId = "9595e0fe83fe44819839bd07931738c9"
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

		_ = fakeClient
			.FundStatusAsync(
				apiUrl: Arg.Is(configuration.ApiUrl),
				website: Arg.Is(configuration.Website),
				cert: Arg.Is(configuration.Cert),
				transactionId: Arg.Is(query.OrderId),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(true);

		var sut = new Cards365TransferPlayerFundCheckingGetService(
			fakeGameProviderRepository,
			fakePlayerRepository,
			fakeBrandRepository,
			fakeClient);

		var actual = await sut.GetAsync(query);

		Assert.True(actual);
	}
}
