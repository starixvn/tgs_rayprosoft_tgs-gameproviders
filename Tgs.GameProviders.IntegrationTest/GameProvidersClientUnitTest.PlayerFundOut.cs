using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Tgs.GameProviders.Client;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Core.Kaiyuan.ApplicationServices;
using Tgs.ServiceUtility;
using Tgs.GameProviders.IntegrationTest;

namespace Tgs.GameProviders.IntegrationTest;

public partial class GameProvidersClientUnitTest
{
	[Fact]
	public async Task Player_Fund_Out()
	{
		var gameProviderCode = "KY";
		var brandCode = "GPI";
		var playerCode = "test";
		var currencyCode = "CNY";
		var orderId = Guid.NewGuid().ToString("N");
		var amount = 1m;
		var externalGameProviderTransactionId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();

		var web = new TestGameProvidersApplication(builder => _ = builder.ConfigureServices(services =>
			{
				var fakeService = Substitute.For<IGetService<KaiyuanPlayerFundOutQuery, PlayerFundResult>>();

				var query = new KaiyuanPlayerFundOutQuery
				{
					BrandCode = brandCode,
					PlayerCode = playerCode,
					CurrencyCode = currencyCode,
					OrderId = orderId,
					Amount = amount
				};
				var result = new PlayerFundResult(
					GameProviderTransactionId: orderId,
					ExternalGameProviderTransactionId: externalGameProviderTransactionId
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
			brandCode,
			playerCode,
			currencyCode,
			orderId,
			amount);

		Assert.NotNull(actual);
		Assert.Equal(orderId, actual.OrderId);
		Assert.Equal(orderId, actual.GameProviderTransactionId);
		Assert.Equal(externalGameProviderTransactionId, actual.ExternalGameProviderTransactionId);
	}

	[Fact]
	public async Task Player_Fund_Out_HttpRequestException()
	{
		var gameProviderCode = "KY";
		var brandCode = "GPI";
		var playerCode = "test";
		var currencyCode = "CNY";
		var orderId = Guid.NewGuid().ToString("N");
		var amount = 1m;

		var web = new TestGameProvidersApplication(builder => _ = builder.ConfigureServices(services =>
			{
				var fakeService = Substitute.For<IGetService<KaiyuanPlayerFundOutQuery, PlayerFundResult>>();

				var query = new KaiyuanPlayerFundOutQuery
				{
					BrandCode = brandCode,
					PlayerCode = playerCode,
					CurrencyCode = currencyCode,
					OrderId = orderId,
					Amount = amount
				};
				var result = new PlayerFundResult(
					GameProviderTransactionId: DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(),
					ExternalGameProviderTransactionId: DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString()
				);
				_ = fakeService
					.GetAsync(
						query: Arg.Is(query),
						cancellationToken: Arg.Any<CancellationToken>())
					.ThrowsForAnyArgs<Exception>();

				_ = services.AddTransient(_ => fakeService);
			}));

		var httpClient = web.CreateClient();

		var sut = new GameProvidersClient(httpClient);

		_ = await Assert.ThrowsAsync<HttpRequestException>(() => sut
			.PlayerFundOutAsync(
				gameProviderCode,
				brandCode,
				playerCode,
				currencyCode,
				orderId,
				amount)
			.AsTask());
	}
}
