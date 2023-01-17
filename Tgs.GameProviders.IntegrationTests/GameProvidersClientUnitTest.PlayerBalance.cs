using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Tgs.GameProviders.Client;
using Tgs.GameProviders.Core.KY.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.IntegrationTests;

public partial class GameProvidersClientUnitTest
{
    [Test]
    public async Task Player_Balance()
    {
        var fakeService = Substitute.For<IGetService<KYPlayerBalanceQuery, decimal?>>();

        var web = new TestGameProvidersApplication(builder =>
        {
            _ = builder.ConfigureServices(services =>
            {
                _ = services.AddTransient(_ => fakeService);
            });
        });

        var gameProviderCode = "KY";
        var playerCode = "test";

        var query = new KYPlayerBalanceQuery
        {
            PlayerCode = playerCode
        };
        _ = fakeService
            .GetAsync(query)
            .Returns(ValueTask.FromResult<decimal?>(1));

        var httpClient = web.CreateClient();

        var sut = new GameProvidersClient(httpClient);

        var actual = await sut
            .PlayerBalanceAsync(
                gameProviderCode: gameProviderCode,
                playerCode: playerCode)
            .ConfigureAwait(false);

        Assert.IsNotNull(actual);
        Assert.AreEqual(1, actual!);
    }

    [Test]
    public void Player_Balance_Exception()
    {
        var fakeService = Substitute.For<IGetService<KYPlayerBalanceQuery, decimal?>>();

        var web = new TestGameProvidersApplication(builder =>
        {
            _ = builder.ConfigureServices(services =>
            {
                _ = services.AddTransient(_ => fakeService);
            });
        });

        var gameProviderCode = "KY";
        var playerCode = "test";

        var query = new KYPlayerBalanceQuery
        {
            PlayerCode = playerCode
        };
        _ = fakeService
            .GetAsync(query)
            .Returns(ValueTask.FromResult<decimal?>(null));

        var httpClient = web.CreateClient();

        var sut = new GameProvidersClient(httpClient);

        _ = Assert.ThrowsAsync<Exception>(() => sut
            .PlayerBalanceAsync(
                gameProviderCode: gameProviderCode,
                playerCode: playerCode)
            .AsTask());
    }

    [Test]
    public void Player_Balance_HttpRequestException()
    {
        var fakeService = Substitute.For<IGetService<KYPlayerBalanceQuery, decimal?>>();

        var web = new TestGameProvidersApplication(builder =>
        {
            _ = builder.ConfigureServices(services =>
            {
                _ = services.AddTransient(_ => fakeService);
            });
        });

        var gameProviderCode = "KY";
        var playerCode = "test";

        var query = new KYPlayerBalanceQuery
        {
            PlayerCode = playerCode
        };
        _ = fakeService
            .GetAsync(query)
            .ThrowsForAnyArgs<Exception>();

        var httpClient = web.CreateClient();

        var sut = new GameProvidersClient(httpClient);

        _ = Assert.ThrowsAsync<HttpRequestException>(() => sut
            .PlayerBalanceAsync(
                gameProviderCode: gameProviderCode,
                playerCode: playerCode)
            .AsTask());
    }
}
