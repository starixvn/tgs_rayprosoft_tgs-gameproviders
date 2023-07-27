using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameProviders.Client;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Core.AsiaGaming.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.IntegrationTests;

public partial class GameProvidersClientUnitTest
{
    [Fact]
    public async Task Game_Url_By_AsiaGaming()
    {
        var gameType = GameProvidersClientGameType.LiveDealerLobby;
        var gameId = 1;
        var brandCode = "GPI";
        var playerCode = "test";
        var currencyCode = "CNY";
        var loginRecordId = Guid.NewGuid();
        var platformType = GameProvidersClientPlatformType.Desktop;
        var language = "zh-TW";
        var ipAddress = "127.0.0.1";
        var backUrl = new Uri("https://dev-t1.rpgp.cc/");
        var playerBalance = 1m;

        var web = new TestGameProvidersApplication(builder =>
        {
            _ = builder.ConfigureServices(services =>
            {
                var fakeService = Substitute.For<IGetService<AsiaGamingGameUrlQuery, GameUrlResult>>();

                var query = new AsiaGamingGameUrlQuery
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
                    PlayerBalance = playerBalance
                };
                var result = new GameUrlResult
                {
                    Url = new Uri("https://www.google.com/")
                };
                _ = fakeService
                    .GetAsync(
                        query: Arg.Is(query),
                        cancellationToken: Arg.Any<CancellationToken>())
                    .Returns(result);

                _ = services.AddTransient(_ => fakeService);
            });
        });

        var httpClient = web.CreateClient();

        var sut = new GameProvidersClient(httpClient);

        var actual = await sut
            .GameUrlByAsiaGamingAsync(
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
                playerBalance: playerBalance)
            .ConfigureAwait(false);

        Assert.NotNull(actual);
    }
}
