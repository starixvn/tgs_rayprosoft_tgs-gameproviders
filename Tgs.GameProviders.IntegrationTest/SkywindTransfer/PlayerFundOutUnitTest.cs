using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Tgs.GameProviders.Client;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Core.SkywindTransfer.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.IntegrationTest.SkywindTransfer;

public class PlayerFundOutUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var gameProviderCode = GameProviderType.SkywindTransfer;
		var playerCode = "test";
		var orderId = Guid.NewGuid().ToString("N");
		var amount = 1m;
		var externalOrderId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();

		var web = new TestGameProvidersApplication(builder => _ = builder.ConfigureServices(services =>
		{
			var fakeService = Substitute.For<IGetService<SkywindTransferPlayerFundOutQuery, PlayerFundResult>>();

			var query = new SkywindTransferPlayerFundOutQuery
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

		var actual = await sut.PlayerFundOutAsync(
			gameProviderCode,
			playerCode,
			orderId,
			amount);

		Assert.NotNull(actual);
		Assert.Equal(orderId, actual.OrderId);
		Assert.Equal(externalOrderId, actual.ExternalOrderId);
	}
}
