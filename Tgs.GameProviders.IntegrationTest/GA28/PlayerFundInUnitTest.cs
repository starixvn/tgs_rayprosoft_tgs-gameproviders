using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Tgs.GameProviders.Client;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Core.GA28.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.IntegrationTest.GA28;

public class PlayerFundInUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var gameProviderCode = GameProviderType.GA28;
		var playerCode = "test";
		var orderId = Guid.NewGuid().ToString("N");
		var amount = 1m;
		var externalOrderId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();

		var web = new TestGameProvidersApplication(builder => _ = builder.ConfigureServices(services =>
		{
			var fakeService = Substitute.For<IGetService<GA28PlayerFundInQuery, PlayerFundResult>>();

			var query = new GA28PlayerFundInQuery
			{
				PlayerCode = playerCode,
				OrderId = orderId,
				Amount = amount
			};
			var result = new PlayerFundResult(
				OrderId: orderId,
				ExternalOrderId: externalOrderId
			);
			_ = fakeService
				.GetAsync(
					query: Arg.Is(query),
					cancellationToken: Arg.Any<CancellationToken>())
				.Returns(result);

			_ = services.AddTransient(_ => fakeService);
		}));

		var httpClient = web.CreateClient();

		var sut = new GameProvidersClient(httpClient);

		var actual = await sut.PlayerFundInAsync(
			gameProviderCode,
			playerCode,
			orderId,
			amount);

		Assert.NotNull(actual);
		Assert.Equal(orderId, actual.OrderId);
		Assert.Equal(externalOrderId, actual.ExternalOrderId);
	}
}
