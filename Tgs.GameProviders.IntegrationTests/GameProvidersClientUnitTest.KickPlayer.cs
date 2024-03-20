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

namespace Tgs.GameProviders.IntegrationTests;

public partial class GameProvidersClientUnitTest
{
    [Fact]
    public async Task Kick_Player()
    {
        var gameProviderCode = "KY";
        var playerCode = "test";

        var web = new TestGameProvidersApplication(builder => _ = builder.ConfigureServices(services =>
			{
				var fakeService = Substitute.For<ICommandService<KaiyuanKickPlayerCommand>>();

				var command = new KaiyuanKickPlayerCommand
				{
					PlayerCode = playerCode
				};
				var result = new GameUrlResult
				{
					Url = new Uri("https://www.google.com/")
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
            .KickPlayerAsync(
            gameProviderCode: gameProviderCode,
            playerCode: playerCode);
    }

    [Fact]
    public async void Kick_Player_HttpRequestException()
    {
        var gameProviderCode = "KY";
        var playerCode = "test";

        var web = new TestGameProvidersApplication(builder => _ = builder.ConfigureServices(services =>
			{
				var fakeService = Substitute.For<ICommandService<KaiyuanKickPlayerCommand>>();

				var command = new KaiyuanKickPlayerCommand
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
            .KickPlayerAsync(
                gameProviderCode: gameProviderCode,
                playerCode: playerCode)
            .AsTask());
    }
}
