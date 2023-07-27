using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Tgs.GameProviders.Client;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Core.VR.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.IntegrationTests;

public partial class GameProvidersClientUnitTest
{
    [Fact]
    public async Task Player_Fund_Checking_By_VR()
    {
        var brandCode = "GPI";
        var playerCode = "test";
        var currencyCode = "CNY";
        var orderId = Guid.NewGuid().ToString("N");
        var startTime = DateTimeOffset.UtcNow.AddMinutes(-1);
        var endTime = DateTimeOffset.UtcNow;

        var web = new TestGameProvidersApplication(builder =>
        {
            _ = builder.ConfigureServices(services =>
            {
                var fakeService = Substitute.For<IGetService<VRPlayerFundCheckingQuery, PlayerFundCheckingResult>>();

                var query = new VRPlayerFundCheckingQuery
                {
                    BrandCode = brandCode,
                    PlayerCode = playerCode,
                    CurrencyCode = currencyCode,
                    OrderId = orderId,
                    StartTime = startTime,
                    EndTime = endTime
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
            .PlayerFundCheckingByVRAsync(
                 brandCode: brandCode,
                 playerCode: playerCode,
                 currencyCode: currencyCode,
                 orderId: orderId,
                 startTime: startTime,
                 endTime: endTime)
            .ConfigureAwait(false);

        Assert.True(actual);
    }

    [Fact]
    public void Player_Fund_Checking_By_VR_HttpRequestException()
    {
        var brandCode = "GPI";
        var playerCode = "test";
        var currencyCode = "CNY";
        var orderId = Guid.NewGuid().ToString("N");
        var startTime = DateTimeOffset.UtcNow.AddMinutes(-1);
        var endTime = DateTimeOffset.UtcNow;

        var web = new TestGameProvidersApplication(builder =>
        {
            _ = builder.ConfigureServices(services =>
            {
                var fakeService = Substitute.For<IGetService<VRPlayerFundCheckingQuery, PlayerFundCheckingResult>>();

                var query = new VRPlayerFundCheckingQuery
                {
                    BrandCode = brandCode,
                    PlayerCode = playerCode,
                    CurrencyCode = currencyCode,
                    OrderId = orderId,
                    StartTime = startTime,
                    EndTime = endTime
                };
                var result = new PlayerFundCheckingResult
                {
                    OrderStatus = true
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
            .PlayerFundCheckingByVRAsync(
                 brandCode: brandCode,
                 playerCode: playerCode,
                 currencyCode: currencyCode,
                 orderId: orderId,
                 startTime: startTime,
                 endTime: endTime)
             .AsTask());
    }
}
