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
public class JokerGameUrlGetServiceUnitTest
{
    [Test]
    public async Task Normal_Process()
    {
        var fakeService = Substitute.For<IJokerService>();
        var fakeAgentMapper = Substitute.For<IJokerAgentMapper>();
        var fakeApiClient = Substitute.For<IJokerApiClient>();

        var gameUrl = new Uri("http://www.gwp6868.net?token=game_token&game=t656f48j75z6a&mobile=false&lang=en");
        var gameToken = "game_token";
        var gameProviderLanguageCode = "en";

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

        var game = new GameInfo(
            2600001,
            GameStatus.Active,
            "t656f48j75z6a",
            null,
            null,
            null,
            Guid.Parse("2EBFE118-B36B-40FA-85B5-78363470ADDC"),
            GameType.Slot,
            GamePlatformType.Desktop,
            false,
            true,
            false,
            true,
            null,
            "Great_Blue",
            "Great Blue",
            null,
            "JOKER");

        var query = new JokerGameUrlQuery
        {
            GameType = game.GameType,
            GameId = game.GameId,
            BrandCode = "GPI_HKD",
            PlayerCode = "4uf6k014jp5x",
            CurrencyCode = "HKD",
            LoginRecordId = Guid.NewGuid(),
            PlatformType = PlatformType.Desktop,
            Language = "en-US",
            IPAddress = "127.0.0.1",
            BackUrl = new("https://dev-t1.rpgp.cc/")
        };

        var sut = new JokerGameUrlGetService(
            fakeApiClient,
            fakeAgentMapper,
            fakeService);

        _ = fakeService.GetSettingConfigurationAsync().Returns(configuration);
        _ = fakeAgentMapper.GetAgent(
            Arg.Is(configuration.BrandMerchants),
            Arg.Is(query.BrandCode),
            Arg.Is(query.CurrencyCode)).Returns(agent);

        _ = fakeService.GetGameAsync(Arg.Is(game.GameId)).Returns((GameInfo?)game);

        _ = fakeService
             .GetGameProviderLanguageCodeAsync(Arg.Is(query.Language))
             .Returns(gameProviderLanguageCode);

        _ = fakeApiClient
             .GetGameTokenAsync(
                 Arg.Is(configuration.ApiUrl),
                     Arg.Is(query.PlayerCode),
                         Arg.Is(agent.AgentId),
                             Arg.Is(agent.SecretKey))
             .Returns(gameToken);

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.AreEqual(gameUrl, actual.Url);
    }
}