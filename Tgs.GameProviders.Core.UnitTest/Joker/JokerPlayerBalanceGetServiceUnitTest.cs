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
public class JokerPlayerBalanceGetServiceUnitTest
{
    [Test]
    public async Task GetBalance_When_Player_Not_Exist()
    {
        var fakeService = Substitute.For<IJokerService>();
        var fakeGamePlayer = Substitute.For<IJokerGamePlayer>();
        var fakeAgentMapper = Substitute.For<IJokerAgentMapper>();
        var fakeApiClient = Substitute.For<IJokerApiClient>();
        var fakePlayer = Substitute.For<IPlayerRepository>();

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

        var player = new PlayerModel(
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

        var query = new JokerPlayerBalanceQuery
        {
            PlayerCode = player.PlayerCode
        };

        var balance = 100m;

        _ = fakePlayer.GetAsync(player.PlayerCode).Returns(player);
        _ = fakeService.GetSettingConfigurationAsync().Returns(configuration);
        _ = fakeAgentMapper
            .GetAgentByCurrencyCode(
                Arg.Is(configuration.BrandMerchants),
                Arg.Is(player.BrandCode),
                Arg.Is(player.DefaultWalletCurrencyCode))
            .Returns(agent);

        _ = fakeApiClient
            .GetBalanceAsync(
                Arg.Is(configuration.ApiUrl),
                Arg.Is(player.PlayerCode),
                Arg.Is(agent.AgentId),
                Arg.Is(agent.SecretKey))
            .Returns(
                x => (false, null),
                x => (true, 100m));

        _ = fakeGamePlayer
            .CreatePlayerAsync(
                Arg.Is(player.PlayerCode),
                Arg.Is(configuration.ApiUrl),
                Arg.Is(agent))
            .Returns(true);

        var sut = new JokerPlayerBalanceGetService(
            fakeApiClient,
            fakeService,
            fakeGamePlayer,
            fakeAgentMapper,
            fakePlayer);

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

        Assert.AreEqual(balance, actual);

        _ = fakeGamePlayer
            .Received(1)
            .CreatePlayerAsync(
                Arg.Is(player.PlayerCode),
                Arg.Is(configuration.ApiUrl),
                Arg.Is(agent));

        _ = fakeApiClient
            .Received(2)
            .GetBalanceAsync(
                Arg.Is(configuration.ApiUrl),
                Arg.Is(player.PlayerCode),
                Arg.Is(agent.AgentId),
                Arg.Is(agent.SecretKey));
    }

    [Test]
    public async Task Normal_Process()
    {
        var fakeService = Substitute.For<IJokerService>();
        var fakeGamePlayer = Substitute.For<IJokerGamePlayer>();
        var fakeAgentMapper = Substitute.For<IJokerAgentMapper>();
        var fakeApiClient = Substitute.For<IJokerApiClient>();
        var fakePlayer = Substitute.For<IPlayerRepository>();

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

        var player = new PlayerModel(
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

        var balance = 100m;

        var query = new JokerPlayerBalanceQuery
        {
            PlayerCode = player.PlayerCode
        };

        _ = fakePlayer.GetAsync(Arg.Is(player.PlayerCode)).Returns(player);
        _ = fakeService.GetSettingConfigurationAsync().Returns(configuration);
        _ = fakeAgentMapper
            .GetAgentByCurrencyCode(
                Arg.Is(configuration.BrandMerchants),
                Arg.Is(player.BrandCode),
                Arg.Is(player.DefaultWalletCurrencyCode))
            .Returns(agent);

        _ = fakeApiClient
            .GetBalanceAsync(
                Arg.Is(configuration.ApiUrl),
                Arg.Is(player.PlayerCode),
                Arg.Is(agent.AgentId),
                Arg.Is(agent.SecretKey))
            .Returns((true, balance));

        var sut = new JokerPlayerBalanceGetService(
            fakeApiClient,
            fakeService,
            fakeGamePlayer,
            fakeAgentMapper,
            fakePlayer);

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

        Assert.AreEqual(balance, actual);

        _ = fakeGamePlayer
            .DidNotReceive()
            .CreatePlayerAsync(
                Arg.Is(player.PlayerCode),
                Arg.Is(configuration.ApiUrl),
                Arg.Is(agent));
    }
}
