using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Tgs.GameProviders.Client;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Core.KY.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.IntegrationTests;

public partial class GameProvidersClientUnitTest
{
    [Test]
    public async Task Kick_Player()
    {
        var fakeService = Substitute.For<ICommandService<KYKickPlayerCommand>>();

        var web = new TestGameProvidersApplication(builder =>
        {
            _ = builder.ConfigureServices(services =>
            {
                _ = services.AddTransient(_ => fakeService);
            });
        });

        var gameProviderCode = "KY";
        var playerCode = "test";

        var command = new KYKickPlayerCommand
        {
            PlayerCode = playerCode
        };
        var result = new GameUrlResult
        {
            Url = new Uri("https://www.google.com/")
        };
        _ = fakeService
            .ExecuteAsync(command);

        var httpClient = web.CreateClient();

        var sut = new GameProvidersClient(httpClient);

        await sut
            .KickPlayerAsync(
                gameProviderCode: gameProviderCode,
                playerCode: playerCode)
           .ConfigureAwait(false);
    }

    [Test]
    public void Kick_Player_HttpRequestException()
    {
        var fakeService = Substitute.For<ICommandService<KYKickPlayerCommand>>();

        var web = new TestGameProvidersApplication(builder =>
        {
            _ = builder.ConfigureServices(services =>
            {
                _ = services.AddTransient(_ => fakeService);
            });
        });

        var gameProviderCode = "KY";
        var playerCode = "test";

        var command = new KYKickPlayerCommand
        {
            PlayerCode = playerCode
        };
        _ = fakeService
            .ExecuteAsync(command)
            .ThrowsForAnyArgs<Exception>();

        var httpClient = web.CreateClient();

        var sut = new GameProvidersClient(httpClient);

        _ = Assert.ThrowsAsync<HttpRequestException>(() => sut
            .KickPlayerAsync(
                gameProviderCode: gameProviderCode,
                playerCode: playerCode)
            .AsTask());
    }
}
