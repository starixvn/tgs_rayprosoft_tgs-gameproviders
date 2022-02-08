using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using TGS.GameProviders.Core.Saba;
using TGS.GameProviders.Core.Saba.ApplicationServices;
using TGS.GameProviders.Core.Saba.Models;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Core.UnitTests.Saba;

[TestFixture]
public class SabaKickPlayerCommandServiceUnitTest
{
    [Test]
    public async Task Normal_Process_At_Real_Environment()
    {
        var fakeLogger = NullLogger<SabaKickPlayerCommandService>.Instance;
        var fakeService = Substitute.For<ISabaService>();
        var fakeApiClient = Substitute.For<ISabaApiClient>();
        var fakeUtility = Substitute.For<ISabaUtility>();
        var fakePlayerService = Substitute.For<IPlayerService>();

        var configuration = new SabaConfiguration
        {
            ServiceEndpointUrl = new("http://game-saba"),
            ApiUrl = new("http://t829tsa.bw6688.com"),
            OperatorId = "ZhiFeng055",
            VendorId = "zzdaa12nu1",
            SecretKey = "SecretKey",
            IsTestEnvironment = false
        };

        var playerModel = new PlayerInfo(
            Guid.Parse("b5a2c708-ab70-4316-9f4d-5d2ad0e77460"),
            "TGS",
            Guid.Parse("66c5fef5-217b-4929-9a08-72c2479a8f4e"),
            "TGS",
            Guid.Parse("4e86719f-243f-46f6-b010-5c758b7285af"),
            "GPI",
            "4ucmqplxb029",
            "4ucmqplxb029",
            Guid.Parse("36a6cdef-094f-eb11-8d88-70b5e82b6b8d"),
            "CNY",
            PlayerStatus.Enabled);

        var command = new SabaKickPlayerCommand
        {
            PlayerCode = playerModel.PlayerCode
        };

        fakePlayerService.GetCacheAsync(playerModel.PlayerCode).Returns(Task.FromResult(playerModel));
        fakeService.GetSettingConfigurationAsync().Returns(ValueTask.FromResult(configuration));
        fakeUtility.ToSabaPlayerId(playerModel.PlayerCode, configuration.IsTestEnvironment).Returns(playerModel.PlayerCode);

        var sut = new SabaKickPlayerCommandService(
            fakeLogger,
            fakePlayerService,
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
                    PlayerId = playerModel.PlayerCode,
                    VendorId = configuration.VendorId
                });
    }

    [Test]
    public async Task Normal_Process_At_Test_Environment()
    {
        var fakeLogger = NullLogger<SabaKickPlayerCommandService>.Instance;
        var fakeService = Substitute.For<ISabaService>();
        var fakeApiClient = Substitute.For<ISabaApiClient>();
        var fakeUtility = Substitute.For<ISabaUtility>();
        var fakePlayerService = Substitute.For<IPlayerService>();

        var configuration = new SabaConfiguration
        {
            ServiceEndpointUrl = new("http://game-saba"),
            ApiUrl = new("http://t829tsa.bw6688.com"),
            OperatorId = "ZhiFeng055",
            VendorId = "zzdaa12nu1",
            SecretKey = "SecretKey",
            IsTestEnvironment = true
        };

        var playerModel = new PlayerInfo(
            Guid.Parse("b5a2c708-ab70-4316-9f4d-5d2ad0e77460"),
            "TGS",
            Guid.Parse("66c5fef5-217b-4929-9a08-72c2479a8f4e"),
            "TGS",
            Guid.Parse("4e86719f-243f-46f6-b010-5c758b7285af"),
            "GPI",
            "4ucmqplxb029",
            "4ucmqplxb029",
            Guid.Parse("36a6cdef-094f-eb11-8d88-70b5e82b6b8d"),
            "CNY",
            PlayerStatus.Enabled);

        var command = new SabaKickPlayerCommand
        {
            PlayerCode = playerModel.PlayerCode
        };

        var playerId = $"{playerModel.PlayerCode}_test";

        fakePlayerService.GetCacheAsync(playerModel.PlayerCode).Returns(Task.FromResult(playerModel));
        fakeService.GetSettingConfigurationAsync().Returns(ValueTask.FromResult(configuration));
        fakeUtility.ToSabaPlayerId(playerModel.PlayerCode, configuration.IsTestEnvironment).Returns(playerId);

        var sut = new SabaKickPlayerCommandService(
            fakeLogger,
            fakePlayerService,
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
