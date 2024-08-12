using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Tgs.GameProviders.Client;
using Tgs.GameProviders.Core.TFGamingTransfer.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.IntegrationTest.TFGamingTransfer;

public class GameUrlUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var playerCode = "4ucmqplxb029";
		var gameId = 1;
		var loginRecordId = Guid.NewGuid().ToString();
		var language = "zh-CN";
		var platformType = PlatformType.Desktop;
		var ipAddress = "127.0.0.1";
		var backUrl = new Uri("https://dev-t1.rpgp.cc/");
		var expected = "http://localhost/";
		var web = new TestGameProvidersApplication(builder => _ = builder.ConfigureServices(services =>
		{
			var fakeService = Substitute.For<IGetService<TFGamingTransferGameUrlQuery, string>>();

			var query = new TFGamingTransferGameUrlQuery
			{
				PlayerCode = playerCode,
				GameId = gameId,
				LoginRecordId = loginRecordId,
				Language = language,
				PlatformType = PlatformType.Desktop,
				IPAddress = ipAddress,
				BackUrl = backUrl
			};
			_ = fakeService.GetAsync(
				query: Arg.Is(query),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(expected);

			_ = services.AddTransient(_ => fakeService);
		}));

		var httpClient = web.CreateDefaultClient();

		var sut = new GameProvidersClient(httpClient);

		var actual = await sut.GameUrlAsync(gameProviderCode: GameProviderType.TFGamingTransfer, playerCode, gameId, loginRecordId, language, platformType, ipAddress, backUrl);

		Assert.NotNull(actual);
		Assert.Equal(expected, actual.ToString());
	}
}
