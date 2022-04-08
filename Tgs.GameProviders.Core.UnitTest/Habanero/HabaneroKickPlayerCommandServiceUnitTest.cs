using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameProviders.Core.Habanero;
using Tgs.GameProviders.Core.Habanero.ApplicationServices;
using Tgs.GameProviders.Core.Habanero.Models;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameProviders.Core.UnitTest.Habanero;

[TestFixture]
public class HabaneroKickPlayerCommandServiceUnitTest
{
    [Test]
    public async Task Normal_Process()
    {
        var fakeLogger = NullLogger<HabaneroKickPlayerCommandService>.Instance;
        var fakeService = Substitute.For<IHabaneroService>();

        var fakeApiClient = Substitute.For<IHabaneroApiClient>();

        var command = new HabaneroKickPlayerCommand
        {
            PlayerCode = "4ucmqplxb029"
        };

        var playerInfo = new PlayerInfo(
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

        var configuration = new HabaneroConfiguration
        {
            ServiceEndpointUrl = new Uri("http://game-haba"),
            BrandId = "a5df0ada-7b6a-eb11-9889-00155db5435a",
            ApiKey = "349F896E-DBC8-4A9D-963D-EFC921AD19FC",
            ApiUrl = new("https://ws-test.insvr.com"),
            BetHistoryUrl = new("https://app-test.insvr.com")
        };

        var request = new HabaneroLogoutRequest
        {
            ApiKey = configuration.ApiKey,
            BrandId = configuration.BrandId,
            PlayerCode = playerInfo.PlayerCode
        };

        var sut = new HabaneroKickPlayerCommandService(
            fakeLogger,
            fakeService,
            fakeApiClient);

        _ = fakeService.GetSettingConfigurationAsync().Returns(ValueTask.FromResult(configuration));
        _ = fakeService.GetPlayerAsync(command.PlayerCode).Returns(ValueTask.FromResult(playerInfo));

        await sut.ExecuteAsync(command).ConfigureAwait(false);

        _ = fakeApiClient.Received(1)
            .LogoutAsync(
                configuration.ApiUrl,
                request);
    }
}
