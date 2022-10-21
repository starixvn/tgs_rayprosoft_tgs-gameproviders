using System;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ReceivedExtensions;
using NUnit.Framework;
using Tgs.GameInterface.WMCasinoService.Client;
using Tgs.GameProviders.Core.WMCasinoTransfer.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameProviders.Core.UnitTest.WMCasinoTransfer;

[TestFixture]
public class WMCasinoTransferKickPlayerCommandServiceUnitTest
{
    [Test]
    public async Task Normal_Process()
    {
        var fakeBrandService = Substitute.For<IBrandRepository>();
        var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
        var fakeGameInterfaceWMCasinoServiceGrpc = Substitute.For<IGameInterfaceWMCasinoServiceGrpc>();
        var fakePlayerRepository = Substitute.For<IPlayerRepository>();
        var fakeSettingRepository = Substitute.For<ISettingRepository>();

        var configuration = new WMCasinoTransferConfiguration
        {
            ApiUrl = new Uri("https://api.a45.me/api/public/Gateway.php"),
            VendorId = "testVendorId",
            Signature = "testsignature"
        };

        var playerModel = new PlayerModel(
            ResellerId: Guid.NewGuid(),
            ResellerCode: "GPI",
            LicenseeId: Guid.NewGuid(),
            LicenseeCode: "GPI",
            BrandId: Guid.NewGuid(),
            BrandCode: "GPI",
            PlayerCode: "4ucmqplxb029",
            Username: "4ucmqplxb029",
            DefaultWalletId: Guid.NewGuid(),
            DefaultWalletCurrencyCode: "CNY",
            PlayerStatus.Enabled);

        var command = new WMCasinoTransferKickPlayerCommand
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
        _ = fakeGameProviderRepository.GetWMCasinoTransferConfiguration(Arg.Is(gameProviderConfigurationId)).Returns(configuration);

        var sut = new WMCasinoTransferKickPlayerCommandService(
            fakeBrandService,
            fakeGameProviderRepository,
            fakeGameInterfaceWMCasinoServiceGrpc,
            fakePlayerRepository,
            fakeSettingRepository);

        await sut.ExecuteAsync(command).ConfigureAwait(false);

        var user = $"wmtf_{playerModel.PlayerCode}_{playerModel.BrandCode}";

        _ = fakeGameInterfaceWMCasinoServiceGrpc
            .Received(1)
            .KickPlayerAsync(
            Arg.Is(configuration.ApiUrl),
            Arg.Is(configuration.VendorId),
            Arg.Is(configuration.Signature),
            Arg.Is(user));
    }
}
