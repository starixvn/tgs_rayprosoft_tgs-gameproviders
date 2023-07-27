using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.GameProviders.Core.Saba;
using Tgs.GameProviders.Core.Saba.ApplicationServices;
using Tgs.GameProviders.Core.Saba.Models;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.Saba;

public class SabaKickPlayerCommandServiceUnitTest
{
    [Fact]
    public async Task Normal_Process_At_Real_Environment()
    {
        var fakeLogger = NullLogger<SabaKickPlayerCommandService>.Instance;
        var fakeService = Substitute.For<ISabaService>();
        var fakeApiClient = Substitute.For<ISabaApiClient>();
        var fakeUtility = Substitute.For<ISabaUtility>();

        var configuration = new SabaConfiguration
        {
            ServiceEndpointUrl = new("http://game-saba"),
            ApiUrl = new("http://t829tsa.bw6688.com"),
            OperatorId = "ZhiFeng055",
            VendorId = "zzdaa12nu1",
            SecretKey = "SecretKey",
            IsTestEnvironment = false
        };

        var command = new SabaKickPlayerCommand
        {
            PlayerCode = "4ucmqplxb029"
        };

        fakeService.GetSettingConfigurationAsync().Returns(ValueTask.FromResult(configuration));
        fakeUtility.ToSabaPlayerId(command.PlayerCode, configuration.IsTestEnvironment).Returns(command.PlayerCode);

        var sut = new SabaKickPlayerCommandService(
            fakeLogger,
            fakeService,
            fakeUtility,
            fakeApiClient);

        await sut.ExecuteAsync(command).ConfigureAwait(false);

        _ = fakeApiClient
            .Received(1)
            .KickUserAsync(
                configuration.ApiUrl,
                new SabaKickUserRequest
                {
                    PlayerId = command.PlayerCode,
                    VendorId = configuration.VendorId
                });
    }

    [Fact]
    public async Task Normal_Process_At_Test_Environment()
    {
        var fakeLogger = NullLogger<SabaKickPlayerCommandService>.Instance;
        var fakeService = Substitute.For<ISabaService>();
        var fakeApiClient = Substitute.For<ISabaApiClient>();
        var fakeUtility = Substitute.For<ISabaUtility>();

        var configuration = new SabaConfiguration
        {
            ServiceEndpointUrl = new("http://game-saba"),
            ApiUrl = new("http://t829tsa.bw6688.com"),
            OperatorId = "ZhiFeng055",
            VendorId = "zzdaa12nu1",
            SecretKey = "SecretKey",
            IsTestEnvironment = true
        };

        var command = new SabaKickPlayerCommand
        {
            PlayerCode = "4ucmqplxb029"
        };

        var playerId = $"{command.PlayerCode}_test";

        fakeService.GetSettingConfigurationAsync().Returns(ValueTask.FromResult(configuration));
        fakeUtility.ToSabaPlayerId(command.PlayerCode, configuration.IsTestEnvironment).Returns(playerId);

        var sut = new SabaKickPlayerCommandService(
            fakeLogger,
            fakeService,
            fakeUtility,
            fakeApiClient);

        await sut.ExecuteAsync(command).ConfigureAwait(false);

        _ = fakeApiClient
            .Received(1)
            .KickUserAsync(
                configuration.ApiUrl,
                new SabaKickUserRequest
                {
                    PlayerId = playerId,
                    VendorId = configuration.VendorId
                });
    }
}
