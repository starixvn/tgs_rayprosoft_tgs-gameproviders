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
	public async Task Kick_Player()
	{
		var gameProviderCode = "KY";
		var playerCode = "test";

		var web = new TestGameProvidersApplication(builder => _ = builder.ConfigureServices(services =>
			{
				var fakeService = Substitute.For<ICommandService<KaiyuanPlayerLogoutCommand>>();

				var command = new KaiyuanPlayerLogoutCommand
				{
					PlayerCode = playerCode
				};

				_ = fakeService
				   .ExecuteAsync(
					   command: Arg.Is(command),
					   cancellationToken: Arg.Any<CancellationToken>());

				_ = services.AddTransient(_ => fakeService);
			}));

		var httpClient = web.CreateClient();

		var sut = new GameProvidersClient(httpClient);

		await sut
			.PlayerLogoutAsync(
			gameProviderCode: gameProviderCode,
			playerCode: playerCode);
	}

	[Fact]
	public async Task Kick_Player_HttpRequestException()
	{
		var gameProviderCode = "KY";
		var playerCode = "test";

		var web = new TestGameProvidersApplication(builder => _ = builder.ConfigureServices(services =>
			{
				var fakeService = Substitute.For<ICommandService<KaiyuanPlayerLogoutCommand>>();

				var command = new KaiyuanPlayerLogoutCommand
				{
					PlayerCode = playerCode
				};
				_ = fakeService
					.ExecuteAsync(
						command: command,
						cancellationToken: Arg.Any<CancellationToken>())
					.ThrowsForAnyArgs<Exception>();

				_ = services.AddTransient(_ => fakeService);
			}));

		var httpClient = web.CreateClient();

		var sut = new GameProvidersClient(httpClient);

		_ = await Assert.ThrowsAsync<HttpRequestException>(() => sut
			.PlayerLogoutAsync(
				gameProviderCode: gameProviderCode,
				playerCode: playerCode)
			.AsTask());
	}
}
