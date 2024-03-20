using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Tgs.GameProviders.Client;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Core.Cards365Transfer.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.IntegrationTests.Cards365Transfer;

public class PlayerBalanceUnitTest
{
    [Fact]
    public async Task GetBalances()
    {
        var playerCode = "4ucmqplxb029";

        var web = new TestGameProvidersApplication(builder => _ = builder.ConfigureServices(services =>
			{
				var fakeService = Substitute.For<IGetService<Cards365TransferPlayerBalanceQuery, PlayerBalanceResult>>();

				var query = new Cards365TransferPlayerBalanceQuery
				{
					PlayerCode = playerCode
				};
				var result = new PlayerBalanceResult
				{
					FreeBalance = 99m,
					OutstandingBalance = 1m
				};

				_ = fakeService
				.GetAsync(
					query: Arg.Is(query),
					cancellationToken: Arg.Any<CancellationToken>())
				.Returns(result);

				_ = services.AddTransient(_ => fakeService);
			}));

        var fakeLoggingHandler = new LoggingHandler(web.Services.GetRequiredService<ILogger<LoggingHandler>>());

        var httpClient = web.CreateDefaultClient([fakeLoggingHandler]);

        var sut = new GameProvidersClient(httpClient);

        var actual = await sut
            .PlayerBalanceAsync(
            gameProviderCode: GameProviderType.Cards365Transfer,
            playerCode: playerCode);

        Assert.NotNull(actual);
        Assert.Equal(100m, actual.Balance);
        Assert.Equal(99m, actual.FreeBalance);
        Assert.Equal(1m, actual.OutstandingBalance);
    }
}
