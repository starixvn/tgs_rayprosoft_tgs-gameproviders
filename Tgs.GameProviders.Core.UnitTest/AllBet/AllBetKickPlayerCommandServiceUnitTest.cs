using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameProviders.Core.AllBet;
using Tgs.GameProviders.Core.AllBet.ApplicationServices;
using Tgs.GameProviders.Core.AllBet.Models;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameProviders.Domains.SettingConfiguration.AllBet;

namespace Tgs.GameProviders.Core.UnitTest.AllBet;

public class AllBetKickPlayerCommandServiceUnitTest
{
    [Fact]
    public async Task Normal_Process()
    {
        var fakeLogger = NullLogger<AllBetKickPlayerCommandService>.Instance;
        var fakeService = Substitute.For<IAllBetService>();
        var fakeApiClient = Substitute.For<IAllBetApiClient>();
        var fakeAgentMapper = Substitute.For<IAllBetAgentMapper>();
        var fakePlayer = Substitute.For<IPlayerRepository>();

        var agent = new AllBetAgent
        {
            PropertyId = "4222910",
            AgentId = "ygpg1a",
            CurrencyCode = "CNY",
            AllBetApiKey =
                "VO5ajdrJrayBQwaEjlMMVmYsME1n38q8Mo5COHKLdymAxP0TQ8Rmq2KdWwly6SlOPKbz4GBM0EDtnE+c6DtVrA==",
            PartnerApiKey =
                "wqRXtqmTssY6dsMb6y7janayZosdF2+KIOZx8G0UWeNOIl+B1+hn56y/unBBR2OUBhgyvxo7sdD9RQt5C9TDkg==",
            Suffix = "mkb"
        };

        var configuration = new AllBetConfiguration
        {
            ServiceEndpointUrl = new("http://game-ab"),
            ApiUrl = new("https://sw2.apidemo.net:8443/"),
            BrandMerchants = new AllBetBrandMerchant[]
            {
                new()
                {
                    BrandCode = DefConst.BrandCodeDefault,
                    Agents = new[] { agent }
                }
            }
        };

        var player = new PlayerInfo(
            Guid.Parse("b5a2c708-ab70-4316-9f4d-5d2ad0e77460"),
            "TGS",
            Guid.Parse("66c5fef5-217b-4929-9a08-72c2479a8f4e"),
            "TGS",
            Guid.Parse("4e86719f-243f-46f6-b010-5c758b7285af"),
            "GPI",
            "4ucmqplxb029",
            "4ucmqplxb029",
            Guid.Parse("36a6cdef-094f-eb11-8d88-70b5e82b6b8d"),
            "CNY");

        var allBetPlayerId = $"{player.PlayerCode}_{agent.Suffix}";

        var command = new AllBetKickPlayerCommand
        {
            PlayerCode = player.PlayerCode
        };

        _ = fakePlayer.GetAsync(player.PlayerCode).Returns(ValueTask.FromResult(player));
        _ = fakeService.GetSettingConfigurationAsync().Returns(ValueTask.FromResult(configuration));
        _ = fakeService.ToAllBetPlayerId(player.PlayerCode, agent.Suffix).Returns(allBetPlayerId);

        _ = fakeAgentMapper
             .GetAgent(
                 configuration.BrandMerchants,
                 player.BrandCode,
                 player.DefaultWalletCurrencyCode)
             .Returns(agent);

        var sut = new AllBetKickPlayerCommandService(
            fakeLogger,
            fakePlayer,
            fakeService,
            fakeAgentMapper,
            fakeApiClient);

        await sut.ExecuteAsync(command);

        _ = fakeApiClient
            .Received(1)
            .LogoutAsync(
                configuration.ApiUrl,
                agent.PropertyId,
                agent.AllBetApiKey,
                new AllBetLogoutRequest
                {
                    PlayerId = allBetPlayerId
                });
    }
}
