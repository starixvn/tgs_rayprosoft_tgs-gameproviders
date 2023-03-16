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
    public async Task Player_Fund_In()
    {
        var gameProviderCode = "KY";
        var brandCode = "GPI";
        var playerCode = "test";
        var currencyCode = "CNY";
        var orderId = Guid.NewGuid().ToString("N");
        var amount = 1m;

        var web = new TestGameProvidersApplication(builder =>
        {
            _ = builder.ConfigureServices(services =>
            {
                var fakeService = Substitute.For<IGetService<KYPlayerFundInQuery, PlayerFundResult>>();

                var query = new KYPlayerFundInQuery
                {
                    BrandCode = brandCode,
                    PlayerCode = playerCode,
                    CurrencyCode = currencyCode,
                    OrderId = orderId,
                    Amount = amount
                };
                var result = new PlayerFundResult
                {
                    OrderId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString()
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
            .PlayerFundInAsync(
                gameProviderCode,
                brandCode,
                playerCode,
                currencyCode,
                orderId,
                amount)
            .ConfigureAwait(false);

        Assert.IsNotNull(actual);
    }

    [Test]
    public void Player_Fund_In_HttpRequestException()
    {
        var gameProviderCode = "KY";
        var brandCode = "GPI";
        var playerCode = "test";
        var currencyCode = "CNY";
        var orderId = Guid.NewGuid().ToString("N");
        var amount = 1m;

        var web = new TestGameProvidersApplication(builder =>
        {
            _ = builder.ConfigureServices(services =>
            {
                var fakeService = Substitute.For<IGetService<KYPlayerFundInQuery, PlayerFundResult>>();

                var query = new KYPlayerFundInQuery
                {
                    BrandCode = brandCode,
                    PlayerCode = playerCode,
                    CurrencyCode = currencyCode,
                    OrderId = orderId,
                    Amount = amount
                };
                var result = new PlayerFundResult
                {
                    OrderId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString()
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
            .PlayerFundInAsync(
                gameProviderCode,
                brandCode,
                playerCode,
                currencyCode,
                orderId,
                amount)
            .AsTask());
    }
}
