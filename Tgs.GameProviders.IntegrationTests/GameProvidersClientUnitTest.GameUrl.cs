using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Tgs.GameCommand.Common;
using Tgs.GameProviders.Client;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Core.KY.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.IntegrationTests;

public partial class GameProvidersClientUnitTest
{
    [Test]
    public async Task Game_Url()
    {
        var fakeService = Substitute.For<IGetService<KYGameUrlQuery, GameUrlResult>>();

        var web = new TestGameProvidersApplication(builder =>
        {
            _ = builder.ConfigureServices(services =>
            {
                _ = services.AddTransient(_ => fakeService);
            });
        });

        var gameProviderCode = "KY";  // en-us
        var gameType = GameProvidersClientGameType.CardGameLobby;
        var gameId = 1;
        var brandCode = "GPI";
        var playerCode = "test";
        var currencyCode = "CNY";
        var loginRecordId = Guid.NewGuid();
        var platformType = GameProvidersClientPlatformType.Desktop;
        var language = "zh-TW";
        var ipAddress = "127.0.0.1";
        var backUrl = new Uri("https://dev-t1.rpgp.cc/");

        var query = new KYGameUrlQuery
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
            BackUrl = backUrl
        };
        var result = new GameUrlResult
        {
            Url = new Uri("https://www.google.com/")
        };
        _ = fakeService
            .GetAsync(query)
            .Returns(result);

        var httpClient = web.CreateClient();

        var sut = new GameProvidersClient(httpClient);

        var actual = await sut
            .GameUrlAsync(
                gameProviderCode: gameProviderCode,
                gameType: gameType,
                gameId: gameId,
                brandCode: brandCode,
                playerCode: playerCode,
                currencyCode: currencyCode,
                loginRecordId: loginRecordId,
                platformType: platformType,
                language: language,
                ipAddress: ipAddress,
                backUrl: backUrl)
            .ConfigureAwait(false);

        Assert.IsNotNull(actual);
        Assert.AreEqual(result.Url, actual!);
    }

    [Test]
    public void Game_Url_HttpRequestException()
    {
        var fakeService = Substitute.For<IGetService<KYGameUrlQuery, GameUrlResult>>();

        var web = new TestGameProvidersApplication(builder =>
        {
            _ = builder.ConfigureServices(services =>
            {
                _ = services.AddTransient(_ => fakeService);
            });
        });

        var gameProviderCode = "KY";  // en-us
        var gameType = GameProvidersClientGameType.CardGameLobby;
        var gameId = 1;
        var brandCode = "GPI";
        var playerCode = "test";
        var currencyCode = "CNY";
        var loginRecordId = Guid.NewGuid();
        var platformType = GameProvidersClientPlatformType.Desktop;
        var language = "zh-TW";
        var ipAddress = "127.0.0.1";
        var backUrl = new Uri("https://dev-t1.rpgp.cc/");

        var query = new KYGameUrlQuery
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
            BackUrl = backUrl
        };
        _ = fakeService
            .GetAsync(query)
            .ThrowsForAnyArgs<Exception>();

        var httpClient = web.CreateClient();

        var sut = new GameProvidersClient(httpClient);

        _ = Assert.ThrowsAsync<HttpRequestException>(() => sut
        .GameUrlAsync(
            gameProviderCode: gameProviderCode,
            gameType: gameType,
            gameId: gameId,
            brandCode: brandCode,
            playerCode: playerCode,
            currencyCode: currencyCode,
            loginRecordId: loginRecordId,
            platformType: platformType,
            language: language,
            ipAddress: ipAddress,
            backUrl: backUrl)
        .AsTask());
    }
}
