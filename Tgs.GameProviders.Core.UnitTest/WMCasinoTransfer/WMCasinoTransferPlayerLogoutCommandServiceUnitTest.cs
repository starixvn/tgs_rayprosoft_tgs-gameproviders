using System;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ReceivedExtensions;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.WMCasinoService.Client;
using Tgs.GameProviders.Core.WMCasinoTransfer.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.WMCasinoTransfer;

public class WMCasinoTransferPlayerLogoutCommandServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeBrandService = Substitute.For<IBrandRepository>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakeClient = Substitute.For<IGameInterfaceWMCasinoServiceClient>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();

		var configuration = new WMCasinoTransferConfiguration
		{
			ApiUrl = new Uri("https://api.a45.me/api/public/Gateway.php"),
			VendorId = "testVendorId",
			Signature = "testsignature"
		};

		var playerModel = new PlayerInfo(
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

		var command = new WMCasinoTransferPlayerLogoutCommand
		{
			PlayerCode = "4ucmqplxb029"
		};

		var gameProviderId = Guid.NewGuid();
		var gameProviderConfigurationId = Guid.NewGuid();
		_ = fakeGameProviderRepository.GetGameProviderIdAsync(Arg.Is(GameProviderType.WMCasinoTransfer)).Returns(gameProviderId);
		_ = fakePlayerRepository.GetAsync(Arg.Is(command.PlayerCode)).Returns(playerModel);
		_ = fakeBrandService.GetBrandGameProviderConfigurationIdAsync(
			 Arg.Is(gameProviderId),
			 Arg.Is(playerModel.BrandId)).Returns(gameProviderConfigurationId);
		_ = fakeGameProviderRepository.GetWMCasinoTransferConfigurationAsync(Arg.Is(gameProviderConfigurationId)).Returns(configuration);

		var sut = new WMCasinoTransferPlayerLogoutCommandService(
			fakeBrandService,
			fakeGameProviderRepository,
			fakeClient,
			fakePlayerRepository);

		await sut.ExecuteAsync(command);

		var user = $"wmtf_{playerModel.PlayerCode}_{playerModel.BrandCode}";

		_ = fakeClient
			.Received(1)
			.KickPlayerAsync(
			Arg.Is(configuration.ApiUrl),
			Arg.Is(configuration.VendorId),
			Arg.Is(configuration.Signature),
			Arg.Is(user));
	}
}
