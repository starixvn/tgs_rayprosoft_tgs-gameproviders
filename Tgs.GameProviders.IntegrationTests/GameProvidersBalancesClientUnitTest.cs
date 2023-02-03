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
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.IntegrationTests;

[TestFixture]
public class GameProvidersBalancesClientUnitTest
{
    [Test]
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
                    query,
                    Arg.Any<CancellationToken>())
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

        Assert.IsNotNull(actual);
        Assert.IsTrue(actual.Any());
    }

    [Test]
    public async Task Test_MarkGameRecordList()
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
                    query,
                    Arg.Any<CancellationToken>())
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

        Assert.IsNotNull(actual);
        Assert.IsTrue(actual.Any());
    }
}
