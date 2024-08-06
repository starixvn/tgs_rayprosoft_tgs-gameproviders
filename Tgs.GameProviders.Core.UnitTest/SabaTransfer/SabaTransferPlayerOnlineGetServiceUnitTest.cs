using System;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.SabaService.Client;
using Tgs.GameProviders.Core.SabaTransfer.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.SabaTransfer;

public class SabaTransferPlayerOnlineGetServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeGameInterfaceSabaClient = Substitute.For<IGameInterfaceSabaClient>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeBrandService = Substitute.For<IBrandRepository>();

		var query = new SabaTransferPlayerOnlineQuery
		{
			PlayerCode = "4ucmqplxb029"
		};

		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProviderRepository.GetGameProviderIdAsync(Arg.Is(GameProviderType.SabaTransfer)).Returns(gameProviderId);

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
		_ = fakePlayerRepository.GetAsync(Arg.Is(query.PlayerCode)).Returns(player);

		var gameProviderConfigurationId = Guid.NewGuid();
		_ = fakeBrandService.GetBrandGameProviderConfigurationIdAsync(
			 Arg.Is(gameProviderId),
			 Arg.Is(player.BrandId)).Returns(gameProviderConfigurationId);

		var configuration = new SabaTransferConfiguration()
		{
			ApiUrl = new("http://localhost/"),
			OperatorId = "tg",
			VendorId = "um5vspnzkx",
			OddsType = 2
		};
		_ = fakeGameProviderRepository.GetSabaTransferConfigurationAsync(Arg.Is(gameProviderConfigurationId)).Returns(configuration);

		_ = fakeGameInterfaceSabaClient.PlayerOnlineAsync(
			apiUrl: Arg.Is(configuration.ApiUrl),
			operatorId: Arg.Is(configuration.OperatorId),
			vendorId: Arg.Is(configuration.VendorId),
			playerCode: Arg.Is(player.PlayerCode))
			.Returns(true);

		var sut = new SabaTransferPlayerOnlineGetService(
			fakeGameInterfaceSabaClient,
			fakeGameProviderRepository,
			fakePlayerRepository,
			fakeBrandService);

		var actual = await sut.GetAsync(query);

		Assert.True(actual);
	}
}
