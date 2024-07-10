using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Tgs.GameProviders.Client;
using Tgs.GameProviders.Core.GTExpanse.ApplicationServices;

using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.IntegrationTest.GTExpanse;

public class PlayerLogoutUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var playerCode = "4ucmqplxb029";

		var fakeService = Substitute.For<ICommandService<GTExpansePlayerLogoutCommand>>();
		var web = new TestGameProvidersApplication(builder => _ = builder.ConfigureServices(services => _ = services.AddTransient(_ => fakeService)));

		var httpClient = web.CreateDefaultClient();

		var sut = new GameProvidersClient(httpClient);

		var actual = await sut
			.PlayerLogoutAsync(
			gameProviderCode: GameProviderType.GT_Expanse,
			playerCode: playerCode);

		Assert.NotNull(actual);
		Assert.True(actual.GameProviderSupport);

		_ = fakeService
			.Received(1)
			.ExecuteAsync(
			command: Arg.Is(new GTExpansePlayerLogoutCommand
			{
				PlayerCode = playerCode
			}),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
