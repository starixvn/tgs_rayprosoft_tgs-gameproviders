using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameProviders.Core.JDB;
using Tgs.GameProviders.Core.JDB.ApplicationServices;
using Tgs.GameProviders.Core.JDB.Models;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameProviders.Domains.SettingConfiguration.JDB;
using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameProviders.Core.UnitTest.JDB;

[TestFixture]
public class JDBKickPlayerCommandServiceUnitTest
{
    [Test]
    public async Task Normal_Process()
    {
        var fakeLogger = NullLogger<JDBKickPlayerCommandService>.Instance;
        var fakePlayerService = Substitute.For<IPlayerService>();
        var fakeService = Substitute.For<IJDBService>();
        var fakeAgentMapper = Substitute.For<IJDBAgentMapper>();
        var fakeApiClient = Substitute.For<IJDBApiClient>();

        var agent = new JDBAgent
        {
            Id = "wgvb3rmb",
            CurrencyCode = "CNY"
        };

        var configuration = new JDBConfiguration
        {
            ServiceEndpointUrl = new("http://game-jdb"),
            ApiUrl = new("http://api.jdb711.com"),
            AESIv = "756edb2fc85fe908",
            AESKey = "4f09f467e6a8fb9f",
            ClientDomain = "VB3",
            TimezoneHour = 8,
            MoreGame = false,
            Agents = new[]
            {
                agent
            }
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

        var command = new JDBKickPlayerCommand
        {
            PlayerCode = playerInfo.PlayerCode
        };

        fakeService.GetSettingConfigurationAsync().Returns(ValueTask.FromResult(configuration));
        fakeAgentMapper.GetAgent(configuration.Agents, playerInfo.DefaultWalletCurrencyCode)
            .Returns(agent);

        fakePlayerService.GetCacheAsync(command.PlayerCode).Returns(Task.FromResult(playerInfo));

        var sut = new JDBKickPlayerCommandService(
            fakeLogger,
            fakePlayerService,
            fakeService,
            fakeAgentMapper,
            fakeApiClient);

        await sut.ExecuteAsync(command).ConfigureAwait(false);

        _ = fakeApiClient
            .Received(1)
            .LogoutAsync(
                configuration.ApiUrl,
                configuration.ClientDomain,
                configuration.AESIv,
                configuration.AESKey,
                Arg.Is<JDBLogoutRequest>(x
                    => x.Parent == agent.Id &&
                       x.UserId == playerInfo.PlayerCode));
    }
}
