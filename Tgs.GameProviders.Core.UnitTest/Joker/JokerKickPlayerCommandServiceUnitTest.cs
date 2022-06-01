using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameProviders.Core.Joker;
using Tgs.GameProviders.Core.Joker.ApplicationServices;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameProviders.Domains.SettingConfiguration.Joker;
using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameProviders.Core.UnitTest.Joker;

[TestFixture]
public class JokerKickPlayerCommandServiceUnitTest
{
    [Test]
    public async Task Normal_Process()
    {
        var fakeService = Substitute.For<IJokerService>();
        var fakeAgentMapper = Substitute.For<IJokerAgentMapper>();
        var fakeApiClient = Substitute.For<IJokerApiClient>();
        var fakePlayerService = Substitute.For<IPlayerService>();

        var agent = new JokerAgent
        {
            AgentId = "TGVY",
            CurrencyCode = "HKD",
            SecretKey = "s5ghrn77baoto",
            Status = true
        };

        var configuration = new JokerConfiguration
        {
            ServiceEndpointUrl = new("http://game-joker"),
            ApiUrl = new("https://w.api788.net"),
            GameLaunchUrl = new Uri("http://www.gwp6868.net/"),
            BrandMerchants = new JokerBrandMerchant[]
            {
                new()
                {
                    BrandCode = "Default",
                    Agents = new[] { agent }
                }
            }
        };

        var playerInfo = new PlayerInfo(
            Guid.Parse("b5a2c708-ab70-4316-9f4d-5d2ad0e77460"),
            "TGS",
            Guid.Parse("66c5fef5-217b-4929-9a08-72c2479a8f4e"),
            "TGS",
            Guid.Parse("9f1ecf19-38cd-474f-b0d2-93638dc07d21"),
            "GPI_HKD",
            "4uf6k014jp5x",
            "4uf6k014jp5x",
            Guid.Parse("cf99c783-acdf-48cc-b1cc-08d99a806de9"),
            "HKD",
            PlayerStatus.Enabled);

        var command = new JokerKickPlayerCommand
        {
            PlayerCode = playerInfo.PlayerCode
        };

        _ = fakePlayerService.GetCacheAsync(Arg.Is(playerInfo.PlayerCode)).Returns(playerInfo);
        _ = fakeService.GetSettingConfigurationAsync().Returns(configuration);

        _ = fakeAgentMapper
            .GetAgentByCurrencyCode(
                Arg.Is(configuration.BrandMerchants),
                Arg.Is(playerInfo.BrandCode),
                Arg.Is(playerInfo.DefaultWalletCurrencyCode))
            .Returns(agent);

        var sut = new JokerKickPlayerCommandService(
            fakeApiClient,
            fakeService,
            fakeAgentMapper,
            fakePlayerService);

        await sut.ExecuteAsync(command).ConfigureAwait(false);

        _ = fakeApiClient
            .Received(1)
            .KickPlayerAsync(
                Arg.Is(configuration.ApiUrl),
                Arg.Is(playerInfo.PlayerCode),
                Arg.Is(agent.AgentId),
                Arg.Is(agent.SecretKey));
    }
}
