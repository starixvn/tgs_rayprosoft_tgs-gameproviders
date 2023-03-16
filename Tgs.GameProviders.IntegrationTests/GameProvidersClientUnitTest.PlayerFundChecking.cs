using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Tgs.GameProviders.Client;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Core.KY.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.IntegrationTests;

public partial class GameProvidersClientUnitTest
{
    [Test]
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
                var fakeService = Substitute.For<IGetService<KYPlayerFundCheckingQuery, PlayerFundCheckingResult>>();

                var query = new KYPlayerFundCheckingQuery
                {
                    BrandCode = brandCode,
                    PlayerCode = playerCode,
                    CurrencyCode = currencyCode,
                    OrderId = orderId
                };
                var result = new PlayerFundCheckingResult
                {
                    OrderStatus = true
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
            .PlayerFundCheckingAsync(
                 gameProviderCode: gameProviderCode,
                 brandCode: brandCode,
                 playerCode: playerCode,
                 currencyCode: currencyCode,
                 orderId: orderId)
            .ConfigureAwait(false);

        Assert.IsNotNull(actual);
        Assert.IsTrue(actual);
    }

    [Test]
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
                var fakeService = Substitute.For<IGetService<KYPlayerFundCheckingQuery, PlayerFundCheckingResult>>();

                var query = new KYPlayerFundCheckingQuery
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
