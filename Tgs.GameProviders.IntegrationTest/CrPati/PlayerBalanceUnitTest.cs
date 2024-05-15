using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Tgs.GameProviders.Client;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Core.CrPati.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;
using Tgs.GameProviders.IntegrationTest;

namespace Tgs.GameProviders.IntegrationTest.CrPati;

public class PlayerBalanceUnitTest
{
	[Fact]
	public async Task GetBalances()
	{
		var playerCode = "4ue0ujotpmuu";

		var web = new TestGameProvidersApplication(builder => _ = builder.ConfigureServices(services =>
			{
				var fakeService = Substitute.For<IGetService<CrPatiPlayerBalanceQuery, PlayerBalanceResult>>();

				var query = new CrPatiPlayerBalanceQuery
				{
					PlayerCode = playerCode
				};
				var result = new PlayerBalanceResult
				{
					FreeBalance = 100m,
					OutstandingBalance = 0m
				};

				_ = fakeService
				.GetAsync(
					query: Arg.Is(query),
					cancellationToken: Arg.Any<CancellationToken>())
				.Returns(result);

				_ = services.AddTransient(_ => fakeService);
			}));

		var httpClient = web.CreateDefaultClient();

		var sut = new GameProvidersClient(httpClient);

		var actual = await sut
			.PlayerBalanceAsync(
			gameProviderCode: GameProviderType.CrPati,
			playerCode: playerCode);

		Assert.NotNull(actual);
		Assert.Equal(100m, actual.Balance);
		Assert.Equal(100m, actual.FreeBalance);
		Assert.Equal(0m, actual.OutstandingBalance);
	}
}
