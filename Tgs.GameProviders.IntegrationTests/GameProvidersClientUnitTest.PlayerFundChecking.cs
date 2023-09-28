using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Tgs.GameProviders.Client;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Core.Kaiyuan.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.IntegrationTests;

public partial class GameProvidersClientUnitTest
{
    [Fact]
    public async Task Player_Fund_Checking()
    {
        var gameProviderCode = "KY";
        var brandCode = "GPI";
        var playerCode = "test";
        var currencyCode = "CNY";
        var orderId = Guid.NewGuid().ToString("N");

        var web = new TestGameProvidersApplication(builder =>
        {
            _ = builder.ConfigureServices(services =>
            {
                var fakeService = Substitute.For<IGetService<KaiyuanPlayerFundCheckingQuery, bool>>();

                var query = new KaiyuanPlayerFundCheckingQuery
                {
                    BrandCode = brandCode,
                    PlayerCode = playerCode,
                    CurrencyCode = currencyCode,
                    OrderId = orderId
                };
                _ = fakeService
                    .GetAsync(
                        query: Arg.Is(query),
                        cancellationToken: Arg.Any<CancellationToken>())
                    .Returns(true);

                _ = services.AddTransient(_ => fakeService);
            });
        });

        var httpClient = web.CreateClient();

        var sut = new GameProvidersClient(httpClient);

        var actual = await sut
            .PlayerFundCheckingAsync(
            gameProviderCode: gameProviderCode,
            brandCode: brandCode,
            playerCode: playerCode,
            currencyCode: currencyCode,
            orderId: orderId);

        Assert.True(actual);
    }

    [Fact]
    public void Player_Fund_HttpRequestException()
    {
        var gameProviderCode = "KY";
        var brandCode = "GPI";
        var playerCode = "test";
        var currencyCode = "CNY";
        var orderId = Guid.NewGuid().ToString("N");

        var web = new TestGameProvidersApplication(builder =>
        {
            _ = builder.ConfigureServices(services =>
            {
                var fakeService = Substitute.For<IGetService<KaiyuanPlayerFundCheckingQuery, bool>>();

                var query = new KaiyuanPlayerFundCheckingQuery
                {
                    BrandCode = brandCode,
                    PlayerCode = playerCode,
                    CurrencyCode = currencyCode,
                    OrderId = orderId
                };

                _ = fakeService
                    .GetAsync(
                        query: Arg.Is(query),
                        cancellationToken: Arg.Any<CancellationToken>())
                    .ThrowsForAnyArgs<Exception>();

                _ = services.AddTransient(_ => fakeService);
            });
        });

        var httpClient = web.CreateClient();

        var sut = new GameProvidersClient(httpClient);

        _ = Assert.ThrowsAsync<HttpRequestException>(() => sut
            .PlayerFundCheckingAsync(
                 gameProviderCode: gameProviderCode,
                 brandCode: brandCode,
                 playerCode: playerCode,
                 currencyCode: currencyCode,
                 orderId: orderId)
            .AsTask());
    }
}
