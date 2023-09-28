using System;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ReceivedExtensions;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.SBOService.Client;
using Tgs.GameProviders.Core.SBOTransfer.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.SBOTransfer;

public class SBOTransferKickPlayerCommandServiceUnitTest
{
    [Fact]
    public async Task Normal_Process()
    {
        var fakeGameInterfaceSBOClient = Substitute.For<IGameInterfaceSBOClient>();
        var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
        var fakePlayerRepository = Substitute.For<IPlayerRepository>();
        var fakeBrandService = Substitute.For<IBrandRepository>();

        var command = new SBOTransferKickPlayerCommand
        {
            PlayerCode = "4ucmqplxb029"
        };

        var gameProviderId = Guid.NewGuid();
        _ = fakeGameProviderRepository.GetGameProviderIdAsync(Arg.Is(GameProviderType.SBOTransfer)).Returns(gameProviderId);

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

        var configuration = new SBOTransferConfiguration()
        {
            ApiUrl = new("http://localhost/"),
            CompanyKey = "companyKey",
            Agent = "Agent",
            Currency = "CNY",
            Min = 1,
            Max = 5000,
            MaxPerMatch = 20000,
            CasinoTableLimit = 1
        };
        _ = fakeGameProviderRepository.GetSBOTransferConfigurationAsync(Arg.Is(gameProviderConfigurationId)).Returns(configuration);

        var sut = new SBOTransferKickPlayerCommandService(
            fakeGameInterfaceSBOClient,
            fakeGameProviderRepository,
            fakePlayerRepository,
            fakeBrandService);

        await sut.ExecuteAsync(command);

        _ = fakeGameInterfaceSBOClient
            .Received(1)
            .PlayerKickAsync(
                Arg.Is(configuration.ApiUrl),
                Arg.Is(configuration.CompanyKey),
                Arg.Is(configuration.Agent),
                Arg.Is(command.PlayerCode));
    }
}
