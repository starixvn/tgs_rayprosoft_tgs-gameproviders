using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameCommand.Common;
using Tgs.GameProviders.Core.Joker;
using Tgs.GameProviders.Core.Joker.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameProviders.Domains.SettingConfiguration.Joker;

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

        var gameInfo = new GameInfo(
            GameId: 1,
            GameCode: "Great_Blue",
            GameProviderId: Guid.NewGuid(),
            GameProviderCode: GameProviderType.Joker,
            GameExternalId: "t656f48j75z6a",
            Type: GameType.Slot,
            PlatformType: GamePlatformType.DesktopAndMobile,
            Name: "Great Blue",
            SubGame: true,
            AvailableToBrand: false,
            HasTurnoverOnTieBet: true,
            Html5: true,
            Description: null,
            EndpointPath: null,
            RuleUrl: null,
            AvailableCurrencies: null,
            Localization: null,
            SubGameProviderCode: GameProviderType.Joker);

        var query = new JokerGameUrlQuery
        {
            GameType = gameInfo.Type,
            GameId = gameInfo.GameId,
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
        _ = fakeAgentMapper.GetAgentByCurrencyCode(
            Arg.Is(configuration.BrandMerchants),
            Arg.Is(query.BrandCode),
            Arg.Is(query.CurrencyCode)).Returns(agent);

        _ = fakeService.GetGameAsync(Arg.Is(gameInfo.GameId)).Returns(gameInfo);

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
