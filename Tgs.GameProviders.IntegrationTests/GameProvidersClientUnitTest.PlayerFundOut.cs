using System;
using System.Net.Http;
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
    public async Task Player_Fund_Out()
    {
        var fakeService = Substitute.For<IGetService<KYPlayerFundOutQuery, PlayerFundResult>>();

        var web = new TestGameProvidersApplication(builder =>
        {
            _ = builder.ConfigureServices(services =>
            {
                _ = services.AddTransient(_ => fakeService);
            });
        });

        var gameProviderCode = "KY";
        var brandCode = "GPI";
        var playerCode = "test";
        var currencyCode = "CNY";
        var orderId = Guid.NewGuid().ToString("N");
        var amount = 1m;

        var query = new KYPlayerFundOutQuery
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
            .GetAsync(query)
            .Returns(result);

        var httpClient = web.CreateClient();

        var sut = new GameProvidersClient(httpClient);

        var actual = await sut
            .PlayerFundOutAsync(
                gameProviderCode,
                brandCode,
                playerCode,
                currencyCode,
                orderId,
                amount)
            .ConfigureAwait(false);

        Assert.IsNotNull(actual);
        Assert.AreEqual(result.OrderId, actual!);
    }

     [Test]
    public void Player_Fund_Out_HttpRequestException()
    {
        var fakeService = Substitute.For<IGetService<KYPlayerFundOutQuery, PlayerFundResult>>();

        var web = new TestGameProvidersApplication(builder =>
        {
            _ = builder.ConfigureServices(services =>
            {
                _ = services.AddTransient(_ => fakeService);
            });
        });

        var gameProviderCode = "KY";
        var brandCode = "GPI";
        var playerCode = "test";
        var currencyCode = "CNY";
        var orderId = Guid.NewGuid().ToString("N");
        var amount = 1m;

        var query = new KYPlayerFundOutQuery
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
            .GetAsync(query)
            .ThrowsForAnyArgs<Exception>();

        var httpClient = web.CreateClient();

        var sut = new GameProvidersClient(httpClient);

        _ = Assert.ThrowsAsync<HttpRequestException>(() => sut
            .PlayerFundOutAsync(
                gameProviderCode,
                brandCode,
                playerCode,
                currencyCode,
                orderId,
                amount)
            .AsTask());
    }
}
