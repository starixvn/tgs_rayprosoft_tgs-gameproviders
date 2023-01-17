using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameProviders.Client;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Core.BetConstruct.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.IntegrationTests;

public partial class GameProvidersClientUnitTest
{
    [Test]
    public async Task Game_Url_By_BetConstruct()
    {
        var fakeService = Substitute.For<IGetService<BetConstructGameUrlQuery, GameUrlResult>>();

        var web = new TestGameProvidersApplication(builder =>
        {
            _ = builder.ConfigureServices(services =>
            {
                _ = services.AddTransient(_ => fakeService);
            });
        });

        var gameType = GameProvidersClientGameType.Sportsbook;
        var gameId = 1;
        var brandCode = "GPI";
        var playerCode = "test";
        var currencyCode = "CNY";
        var loginRecordId = Guid.NewGuid();
        var platformType = GameProvidersClientPlatformType.Desktop;
        var language = "zh-TW";
        var ipAddress = "127.0.0.1";
        var backUrl = new Uri("https://dev-t1.rpgp.cc/");
        var lobbyDomain = "https://devsportsbook.srvttt.com";

        var query = new BetConstructGameUrlQuery
        {
            GameType = (GameType)gameType,
            GameId = gameId,
            BrandCode = brandCode,
            PlayerCode = playerCode,
            CurrencyCode = currencyCode,
            LoginRecordId = loginRecordId,
            PlatformType = (PlatformType)platformType,
            Language = language,
            IPAddress = ipAddress,
            BackUrl = backUrl,
            LobbyDomain = lobbyDomain
        };
        var result = new GameUrlResult
        {
            Url = new Uri("https://www.google.com/")
        };
        _ = fakeService
            .GetAsync(
                Arg.Is(query))
            .Returns(result);

        var httpClient = web.CreateClient();

        var sut = new GameProvidersClient(httpClient);

        var actual = await sut
            .GameUrlByBetConstructAsync(
                gameType: gameType,
                gameId: gameId,
                brandCode: brandCode,
                playerCode: playerCode,
                currencyCode: currencyCode,
                loginRecordId: loginRecordId,
                platformType: platformType,
                language: language,
                ipAddress: ipAddress,
                backUrl: backUrl,
                lobbyDomain: lobbyDomain)
            .ConfigureAwait(false);

        Assert.IsNotNull(actual);
        Assert.AreEqual(result.Url, actual);
    }
}
