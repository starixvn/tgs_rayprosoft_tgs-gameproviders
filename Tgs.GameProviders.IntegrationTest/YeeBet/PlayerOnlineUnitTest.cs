using System.Threading.Tasks;
using Tgs.GameProviders.Client;
using Tgs.GameProviders.Domains;

namespace Tgs.GameProviders.IntegrationTest.YeeBet;

public class PlayerOnlineUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var playerCode = "4ucmqplxb029";

		var web = new TestGameProvidersApplication(builder => _ = builder.ConfigureServices(services => { }));

		var httpClient = web.CreateDefaultClient();

		var sut = new GameProvidersClient(httpClient);

		var actual = await sut.PlayerOnlineAsync(
			gameProviderCode: GameProviderType.YeeBet,
			playerCode: playerCode);

		Assert.NotNull(actual);
		Assert.False(actual.GameProviderSupport);
		Assert.False(actual.Online);
	}
}
