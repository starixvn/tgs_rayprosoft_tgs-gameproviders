using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Tgs.GameProviders.Client;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Core.AwcSg.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.IntegrationTests.AwcSg;

public class PlayerBalanceUnitTest
{
    [Fact]
    public async Task GetBalances()
    {
        var playerCode = "4ucmqplxb029";

        var web = new TestGameProvidersApplication(builder =>
        {
            _ = builder.ConfigureServices(services =>
            {
                var fakeService = Substitute.For<IGetService<AwcSgPlayerBalanceQuery, PlayerBalanceResult>>();

                var query = new AwcSgPlayerBalanceQuery
                {
                    PlayerCode = playerCode
                };
                var result = new PlayerBalanceResult
                {
                    FreeBalance = 99m,
                    OccupyBalance = 1m
                };

                _ = fakeService
                .GetAsync(
                    query: Arg.Is(query),
                    cancellationToken: Arg.Any<CancellationToken>())
                .Returns(result);

                _ = services.AddTransient(_ => fakeService);
            });
        });

        var fakeLoggingHandler = new LoggingHandler(web.Services.GetRequiredService<ILogger<LoggingHandler>>());

        var httpClient = web.CreateDefaultClient(new System.Net.Http.DelegatingHandler[] { fakeLoggingHandler });

        var sut = new GameProvidersClient(httpClient);

        var actual = await sut
            .PlayerBalanceAsync(
            gameProviderCode: GameProviderType.AWC_SG,
            playerCode: playerCode)
            .ConfigureAwait(false);

        Assert.Equal(99m, actual);
    }
}
