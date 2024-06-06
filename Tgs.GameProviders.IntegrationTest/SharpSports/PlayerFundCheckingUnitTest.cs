using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Tgs.GameProviders.Client;
using Tgs.GameProviders.Core.SharpSports.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.IntegrationTest.SharpSports;

public class PlayerFundCheckingUnitTest
{
	[Fact]
	public async Task PlayerFundChecking()
	{
		var gameProviderCode = GameProviderType.SharpSports;
		var playerCode = "test";
		var orderId = Guid.NewGuid().ToString("N");
		var ordeOn = new DateTimeOffset(2024, 06, 03, 11, 18, 03, TimeSpan.FromHours(+8));

		var web = new TestGameProvidersApplication(builder => _ = builder.ConfigureServices(services =>
		{
			var fakeService = Substitute.For<IGetService<SharpSportsPlayerFundCheckingQuery, bool>>();

			var query = new SharpSportsPlayerFundCheckingQuery
			{
				PlayerCode = playerCode,
				OrderId = orderId,
				OrderOn = ordeOn
			};
			_ = fakeService.GetAsync(
				query: Arg.Is(query),
				cancellationToken: Arg.Any<CancellationToken>())
				.Returns(true);

			_ = services.AddTransient(_ => fakeService);
		}));

		var httpClient = web.CreateClient();

		var sut = new GameProvidersClient(httpClient);

		var actual = await sut.PlayerFundCheckingAsync(
			gameProviderCode: gameProviderCode,
			playerCode: playerCode,
			orderId: orderId,
			orderOn: ordeOn);

		Assert.NotNull(actual);
		Assert.True(actual.OrderStatus);
	}
}
