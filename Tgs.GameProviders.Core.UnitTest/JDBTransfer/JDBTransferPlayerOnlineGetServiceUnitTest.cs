using System;
using System.Threading;

using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.JDBService.Client;
using Tgs.GameProviders.Core.JDBTransfer.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.JDBTransfer;

public class JDBTransferPlayerOnlineGetServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeGameInterfaceJDBClient = Substitute.For<IGameInterfaceJDBClient>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeBrandService = Substitute.For<IBrandRepository>();

		var query = new JDBTransferPlayerOnlineQuery
		{
			PlayerCode = "4ucmqplxb029"
		};

		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProviderRepository.GetGameProviderIdAsync(
			Arg.Is(GameProviderType.JDBTransfer),
			Arg.Any<CancellationToken>()).Returns(gameProviderId);

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
		_ = fakeBrandService.GetBrandGameProviderConfigurationIdAsync(
			 gameProviderId: Arg.Is(gameProviderId),
			 brandId: Arg.Is(player.BrandId),
			 cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameProviderConfigurationId);

		var configuration = new JDBTransferConfiguration()
		{
			ApiUrl = new("http://localhost/"),
			Domain = "Domain",
			Agent = "Agent",
			AesIV = "AesIV",
			AesKey = "AesKey"
		};
		_ = fakeGameProviderRepository.GetJDBTransferConfigurationAsync(
			gameProviderConfigurationId: Arg.Is(gameProviderConfigurationId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(configuration);

		_ = fakeGameInterfaceJDBClient.PlayerOnlineAsync(
			apiUrl: Arg.Is(configuration.ApiUrl),
			domain: Arg.Is(configuration.Domain),
			aesKey: Arg.Is(configuration.AesKey),
			aesIV: Arg.Is(configuration.AesIV),
			agent: Arg.Is(configuration.Agent),
			playerCode: Arg.Is(player.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(true);

		var sut = new JDBTransferPlayerOnlineGetService(
			fakeGameProviderRepository,
			fakePlayerRepository,
			fakeBrandService,
			fakeGameInterfaceJDBClient);

		var actual = await sut.GetAsync(query);

		Assert.True(actual);
	}
}
