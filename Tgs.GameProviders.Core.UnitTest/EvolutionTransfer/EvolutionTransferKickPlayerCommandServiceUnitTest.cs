using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameInterface.EvolutionService.Client;
using Tgs.GameProviders.Core.EvolutionTransfer.ApplicationServices;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.EvolutionTransfer;

public class EvolutionTransferKickPlayerCommandServiceUnitTest
{
    [Fact]
    public async Task Normal_Process()
    {
        var fakeGameInterfaceEvolutionServiceGrpc = Substitute.For<IGameInterfaceEvolutionClient>();
        var fakeSettingRepository = Substitute.For<ISettingRepository>();

        var configuration = new EvolutionTransferConfiguration
        {
            Ua2AuthenticationUrl = new("https://babylonpega2.uat1.evo-test.com/ua/v1/babylonpega20001/test123"),
            ApiUrl = new("https://babylonpega2.uat1.evo-test.com"),
            CasinoId = "babylonpega20001",
            Ua2Token = "test123",
            GameHistoryApiToken = "test123",
            ExternalLobbyApiToken = "test123",
            EcId = "babylonpega20001test123"
        };

        var command = new EvolutionTransferKickPlayerCommand
        {
            PlayerCode = "4ucmqplxb029"
        };

        _ = fakeSettingRepository.GetEvolutionTransferConfigurationAsync().Returns(ValueTask.FromResult(configuration));

        var sut = new EvolutionTransferKickPlayerCommandService(
            fakeGameInterfaceEvolutionServiceGrpc,
            fakeSettingRepository);

        await sut.ExecuteAsync(command).ConfigureAwait(false);

        _ = fakeGameInterfaceEvolutionServiceGrpc
            .Received(1)
            .KickPlayerAsync(
            Arg.Is(configuration.ApiUrl),
            Arg.Is(configuration.CasinoId),
            Arg.Is(configuration.Ua2Token),
            Arg.Any<string>(),
            Arg.Is(command.PlayerCode));
    }
}
