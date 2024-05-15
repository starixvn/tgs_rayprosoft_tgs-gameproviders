using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Tgs.GameProviders.Client;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Core.SkywindTransfer.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;
using Tgs.GameProviders.IntegrationTest;

namespace Tgs.GameProviders.IntegrationTest.SkywindTransfer;

public class PlayerBalanceUnitTest
{
	[Fact]
	public async Task GetBalances()
	{
		var playerCode = "4ucmqplxb029";

		var web = new TestGameProvidersApplication(builder => _ = builder.ConfigureServices(services =>
			{
				var fakeService = Substitute.For<IGetService<SkywindTransferPlayerBalanceQuery, PlayerBalanceResult>>();

				var query = new SkywindTransferPlayerBalanceQuery
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

		var httpClient = web.CreateDefaultClient();

		var sut = new GameProvidersClient(httpClient);

		var actual = await sut
			.PlayerBalanceAsync(
			gameProviderCode: GameProviderType.SkywindTransfer,
			playerCode: playerCode);

		Assert.NotNull(actual);
		Assert.Equal(100m, actual.Balance);
		Assert.Equal(99m, actual.FreeBalance);
		Assert.Equal(1m, actual.OutstandingBalance);
	}
}
