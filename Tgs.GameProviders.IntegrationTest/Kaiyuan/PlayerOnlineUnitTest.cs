using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Tgs.GameProviders.Client;
using Tgs.GameProviders.Core.Kaiyuan.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.IntegrationTest.Kaiyuan;

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
			var fakeService = Substitute.For<IGetService<KaiyuanPlayerOnlineQuery, bool>>();

			var query = new KaiyuanPlayerOnlineQuery
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

		var actual = await sut.PlayerOnlineAsync(
			gameProviderCode: GameProviderType.Kaiyuan,
			playerCode: playerCode);

		Assert.NotNull(actual);
		Assert.True(actual.GameProviderSupport);
		Assert.Equal(expected, actual.Online);
	}
}
