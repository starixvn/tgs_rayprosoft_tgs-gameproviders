using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Tgs.GameProviders.Client;
using Tgs.GameProviders.Core.V8Poker.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.IntegrationTest.V8Poker;

public class PlayerOnlineUnitTest
{
	[Theory]
	[InlineData(true, true)]
	[InlineData(false, false)]
	public async Task Normal_Process(bool online, bool expected)
	{
		var playerCode = "4ucmqplxb029";

		var web = new TestGameProvidersApplication(builder => _ = builder.ConfigureServices(services =>
		{
			var fakeService = Substitute.For<IGetService<V8PokerPlayerOnlineQuery, bool>>();

			var query = new V8PokerPlayerOnlineQuery
			{
				PlayerCode = playerCode
			};

			_ = fakeService
			.GetAsync(
				query: Arg.Is(query),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(online);

			_ = services.AddTransient(_ => fakeService);
		}));

		var httpClient = web.CreateDefaultClient();

		var sut = new GameProvidersClient(httpClient);

		var actual = await sut
			.PlayerOnlineAsync(
			gameProviderCode: GameProviderType.V8Poker,
			playerCode: playerCode);

		Assert.NotNull(actual);
		Assert.True(actual.GameProviderSupport);
		Assert.Equal(expected, actual.Online);
	}
}
