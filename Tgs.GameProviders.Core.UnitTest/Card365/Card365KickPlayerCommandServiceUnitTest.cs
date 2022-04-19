using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameProviders.Core.Card365;
using Tgs.GameProviders.Core.Card365.ApplicationServices;
using Tgs.GameProviders.Core.Card365.Model;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameProviders.Domains.SettingConfiguration.Card365;
using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameProviders.Core.UnitTest.Card365;

[TestFixture]
public class Card365KickPlayerCommandServiceUnitTest
{
    [Test]
    public async Task Normal_Process()
    {
        var fakeLogger = NullLogger<Card365KickPlayerCommandService>.Instance;
        var fakeService = Substitute.For<ICard365Service>();
        var fakeApiClient = Substitute.For<ICard365ApiClient>();
        var fakeAgentMapper = Substitute.For<ICard365AgentMapper>();
        var fakePlayerService = Substitute.For<IPlayerService>();

        var agent = new Card365BrandAgent
        {
            BrandCode = CustomConst.BrandCodeDefault,
            AgentId = "card0201"
        };

        var configuration = new Card365Configuration
        {
            ServiceEndpointUrl = new("http://game-365"),
            ApiUrl = new("https://g5ky3i.365gaming.cc/api"),
            Cert = "755C24B710D9F2B4B15DAB2B489CF471",
            Website = "card365pega",
            BrandAgents = new[] { agent }
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

        var command = new Card365KickPlayerCommand
        {
            PlayerCode = playerInfo.PlayerCode
        };

        fakeService.GetSettingConfigurationAsync().Returns(ValueTask.FromResult(configuration));
        fakePlayerService.GetCacheAsync(command.PlayerCode).Returns(Task.FromResult(playerInfo));
        fakeAgentMapper.GetAgent(configuration.BrandAgents, playerInfo.BrandCode).Returns(agent);

        var sut = new Card365KickPlayerCommandService(
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
                 configuration.Website,
                 new Card365LogoutRequest
                 {
                     PlayerCode = playerInfo.PlayerCode,
                     Cert = configuration.Cert,
                     AgentId = agent.AgentId
                 });
    }
}
