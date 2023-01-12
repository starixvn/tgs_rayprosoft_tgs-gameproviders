using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameInterface.DreamGamingService.Client;
using Tgs.GameInterface.DreamGamingService.Protos;
using Tgs.GameProviders.Core.DreamGamingTransfer.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameProviders.Core.UnitTest.DreamGamingTransfer;

[TestFixture]
public class DreamGamingTransferKickPlayerCommandServiceUnitTest
{
    [Test]
    public async Task Normal_Process()
    {
        var fakeBrandService = Substitute.For<IBrandRepository>();
        var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
        var fakePlayerRepository = Substitute.For<IPlayerRepository>();
        var fakeGameInterfaceDreamGamingClient = Substitute.For<IGameInterfaceDreamGamingClient>();

        var command = new DreamGamingTransferKickPlayerCommand() { PlayerCode = "4ucmqplxb029" };

        var gameProviderId = Guid.NewGuid();
        _ = fakeGameProviderRepository.GetGameProviderIdAsync(Arg.Is(GameProviderType.DreamGamingTransfer)).Returns(gameProviderId);

        var player = new PlayerModel(
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
        _ = fakePlayerRepository.GetAsync(Arg.Is(command.PlayerCode)).Returns(player);

        var gameProviderConfigurationId = Guid.NewGuid();
        _ = fakeBrandService.GetBrandGameProviderConfigurationIdAsync(
             Arg.Is(gameProviderId),
             Arg.Is(player.BrandId)).Returns(gameProviderConfigurationId);

        var configuration = new DreamGamingTransferConfiguration()
        {
            ApiUrl = new("https://api.dg0.co/"),
            ApiKey = "d39da9da731b427b9349b80d9d97f2e4",
            AgentName = "DGTE0105IH",
        };
        _ = fakeGameProviderRepository.GetDreamGamingTransferConfiguration(Arg.Is(gameProviderConfigurationId)).Returns(configuration);

        var playerOnlines = new PlayerOnline[]
        {
            new PlayerOnline
            {
                PlayerCode = "4ucmqplxb029",
                MemberId = 94030972,
            }
        };

        _ = fakeGameInterfaceDreamGamingClient
            .PlayerOnlineAsync(configuration.ApiUrl, configuration.ApiKey, configuration.AgentName)
            .Returns(playerOnlines.ToAsyncEnumerable());

        var sut = new DreamGamingTransferKickPlayerCommandService(
            fakeBrandService,
            fakeGameProviderRepository,
            fakeGameInterfaceDreamGamingClient,
            fakePlayerRepository);

        await sut.ExecuteAsync(command).ConfigureAwait(false);

        _ = fakeGameInterfaceDreamGamingClient
            .Received(1)
            .PlayerKickAsync(
                Arg.Is(configuration.ApiUrl),
                Arg.Is(configuration.ApiKey),
                Arg.Is(configuration.AgentName),
                Arg.Is(playerOnlines.FirstOrDefault(x => x.PlayerCode == player.PlayerCode)!.MemberId),
                Arg.Any<CancellationToken>());
    }
}
