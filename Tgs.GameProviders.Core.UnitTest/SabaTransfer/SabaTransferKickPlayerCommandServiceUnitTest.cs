using System;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ReceivedExtensions;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.SabaService.Client;
using Tgs.GameProviders.Core.SabaTransfer.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.SabaTransfer;

public class SabaTransferKickPlayerCommandServiceUnitTest
{
    [Fact]
    public async Task Normal_Process()
    {
        var fakeGameInterfaceSabaClient = Substitute.For<IGameInterfaceSabaClient>();
        var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
        var fakePlayerRepository = Substitute.For<IPlayerRepository>();
        var fakeBrandService = Substitute.For<IBrandRepository>();

        var command = new SabaTransferKickPlayerCommand
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
        _ = fakePlayerRepository.GetAsync(Arg.Is(command.PlayerCode)).Returns(player);

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

        var sut = new SabaTransferKickPlayerCommandService(
            fakeGameInterfaceSabaClient,
            fakeGameProviderRepository,
            fakePlayerRepository,
            fakeBrandService);

        await sut.ExecuteAsync(command).ConfigureAwait(false);

        _ = fakeGameInterfaceSabaClient
            .Received(1)
            .PlayerKickAsync(
                apiUrl: Arg.Is(configuration.ApiUrl),
                operatorId: Arg.Is(configuration.OperatorId),
                vendorId: Arg.Is(configuration.VendorId),
                vendorMemberId: Arg.Is(player.PlayerCode));
    }
}
