using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Tgs.GameProviders.Client;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Core.AwcAelotto.ApplicationServices;
using Tgs.GameProviders.Core.AwcAws.ApplicationServices;
using Tgs.GameProviders.Core.AwcFacai.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.IntegrationTests;

public class GameProvidersBalancesClientUnitTest
{
    [Fact]
    public async Task AwcAelotto_GetBalances()
    {
        var brandCode = "GPI";

        var web = new TestGameProvidersApplication(builder =>
        {
            _ = builder.ConfigureServices(services =>
            {
                var fakeService = Substitute.For<IQueryService<AwcAelottoBalancesQuery, BalanceResult>>();

                var query = new AwcAelottoBalancesQuery(brandCode);

                _ = fakeService
                .QueryAsync(
                    query: Arg.Is(query),
                    cancellationToken: Arg.Any<CancellationToken>())
                .Returns(new BalanceResult[] {
                    new BalanceResult(
                        "test",
                        1m,
                        DateTime.Now)
                }.ToAsyncEnumerable());

                _ = services.AddTransient(_ => fakeService);
            });
        });

        var fakeLoggingHandler = new LoggingHandler(web.Services.GetRequiredService<ILogger<LoggingHandler>>());

        var httpClient = web.CreateDefaultClient(new System.Net.Http.DelegatingHandler[] { fakeLoggingHandler });

        var sut = new GameProvidersBalancesClient(httpClient);

        var actual = await sut
            .AwcAelottoGetBalances(
                brandCode)
            .ToArrayAsync()
            .ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.True(actual.Any());
    }

    [Fact]
    public async Task AwcAws_GetBalances()
    {
        var brandCode = "GPI";

        var web = new TestGameProvidersApplication(builder =>
        {
            _ = builder.ConfigureServices(services =>
            {
                var fakeService = Substitute.For<IQueryService<AwcAwsBalancesQuery, BalanceResult>>();

                var query = new AwcAwsBalancesQuery(brandCode);

                _ = fakeService
                .QueryAsync(
                    query: Arg.Is(query),
                    cancellationToken: Arg.Any<CancellationToken>())
                .Returns(new BalanceResult[] {
                    new BalanceResult(
                        "test",
                        1m,
                        DateTime.Now)
                }.ToAsyncEnumerable());

                _ = services.AddTransient(_ => fakeService);
            });
        });

        var fakeLoggingHandler = new LoggingHandler(web.Services.GetRequiredService<ILogger<LoggingHandler>>());

        var httpClient = web.CreateDefaultClient(new System.Net.Http.DelegatingHandler[] { fakeLoggingHandler });

        var sut = new GameProvidersBalancesClient(httpClient);

        var actual = await sut
            .AwcAwsGetBalances(
                brandCode)
            .ToArrayAsync()
            .ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.True(actual.Any());
    }

    [Fact]
    public async Task AwcFacai_GetBalances()
    {
        var brandCode = "GPI";

        var web = new TestGameProvidersApplication(builder =>
        {
            _ = builder.ConfigureServices(services =>
            {
                var fakeService = Substitute.For<IQueryService<AwcFacaiBalancesQuery, BalanceResult>>();

                var query = new AwcFacaiBalancesQuery(brandCode);

                _ = fakeService
                .QueryAsync(
                    query: Arg.Is(query),
                    cancellationToken: Arg.Any<CancellationToken>())
                .Returns(new BalanceResult[] {
                    new BalanceResult(
                        "test",
                        1m,
                        DateTime.Now)
                }.ToAsyncEnumerable());

                _ = services.AddTransient(_ => fakeService);
            });
        });

        var fakeLoggingHandler = new LoggingHandler(web.Services.GetRequiredService<ILogger<LoggingHandler>>());

        var httpClient = web.CreateDefaultClient(new System.Net.Http.DelegatingHandler[] { fakeLoggingHandler });

        var sut = new GameProvidersBalancesClient(httpClient);

        var actual = await sut
            .AwcFacaiGetBalances(
                brandCode)
            .ToArrayAsync()
            .ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.True(actual.Any());
    }
}
