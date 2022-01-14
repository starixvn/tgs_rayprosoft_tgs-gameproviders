using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using TGS.GameProviders.Core.JDB;
using TGS.GameProviders.Core.JDB.ApplicationServices;
using TGS.GameProviders.Core.JDB.Models;
using TGS.GameProviders.Core.Repositories;
using TGS.GameProviders.Core.Repositories.Models.Player;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.SettingConfiguration;
using TGS.GameProviders.Domains.SettingConfiguration.JDB;

namespace TGS.GameProviders.Core.UnitTests.JDB;

[TestFixture]
public class JDBKickPlayerCommandServiceUnitTest
{
    [Test]
    public async Task Normal_Process()
    {
        var fakeLogger = NullLogger<JDBKickPlayerCommandService>.Instance;
        var fakePlayerRepository = Substitute.For<IPlayerRepository>();
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

        var playerModel = new PlayerModel(
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
            PlayerCode = playerModel.PlayerCode
        };

        fakeService.GetSettingConfigurationAsync().Returns(ValueTask.FromResult(configuration));
        fakeAgentMapper.GetAgent(configuration.Agents, playerModel.DefaultWalletCurrencyCode)
            .Returns(agent);

        fakePlayerRepository.GetAsync(command.PlayerCode).Returns(Task.FromResult(playerModel));

        var sut = new JDBKickPlayerCommandService(
            fakeLogger,
            fakePlayerRepository,
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
                       x.UserId == playerModel.PlayerCode));
    }
}