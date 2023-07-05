using Tgs.GameCommand.Client;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.AwcService.Client;
using Tgs.GameInterface.BTiService.Client;
using Tgs.GameInterface.Cards365.Client;
using Tgs.GameInterface.DreamGamingService.Client;
using Tgs.GameInterface.EvolutionService.Client;
using Tgs.GameInterface.FaChai.Client;
using Tgs.GameInterface.InplayMatrix.Client;
using Tgs.GameInterface.Joker.Client;
using Tgs.GameInterface.OBet.Client;
using Tgs.GameInterface.Pinnacle.Client;
using Tgs.GameProviders.Core;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceRegisterExtensions
{
    public static async Task<WebApplication> InitialConnectionAwc(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var gameInterface = scope.ServiceProvider.GetRequiredService<IGameInterfaceAwcServiceClient>();
            _ = await gameInterface
                .CheckingServiceAsync(
                machineName: Environment.MachineName)
                .ConfigureAwait(false);
        }
        return app;
    }

    public static async Task<WebApplication> InitialConnectionBTi(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var gameInterface = scope.ServiceProvider.GetRequiredService<IGameInterfaceBTiClient>();
            _ = await gameInterface
                .CheckingServiceAsync(
                machineName: Environment.MachineName)
                .ConfigureAwait(false);
        }
        return app;
    }

    public static async Task<WebApplication> InitialConnectionCards365(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var gameInterface = scope.ServiceProvider.GetRequiredService<IGameInterfaceCards365Client>();
            _ = await gameInterface
                .CheckingServiceAsync(
                machineName: Environment.MachineName)
                .ConfigureAwait(false);
        }
        return app;
    }

    public static async Task<WebApplication> InitialConnectionDreamGaming(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var gameInterface = scope.ServiceProvider.GetRequiredService<IGameInterfaceDreamGamingClient>();
            _ = await gameInterface
                .CheckingServiceAsync(
                machineName: Environment.MachineName)
                .ConfigureAwait(false);
        }
        return app;
    }

    public static async Task<WebApplication> InitialConnectionEvolution(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var gameInterface = scope.ServiceProvider.GetRequiredService<IGameInterfaceEvolutionClient>();
            _ = await gameInterface
                .CheckingServiceAsync(
                machineName: Environment.MachineName)
                .ConfigureAwait(false);
        }
        return app;
    }

    public static async Task<WebApplication> InitialConnectionFaChai(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var gameInterface = scope.ServiceProvider.GetRequiredService<IGameInterfaceFaChaiClient>();
            _ = await gameInterface
                .CheckingServiceAsync(
                machineName: Environment.MachineName)
                .ConfigureAwait(false);
        }
        return app;
    }

    public static async Task<WebApplication> InitialConnectionInplayMatrix(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var gameInterface = scope.ServiceProvider.GetRequiredService<IGameInterfaceInplayMatrixClient>();
            _ = await gameInterface
                .CheckingServiceAsync(
                machineName: Environment.MachineName)
                .ConfigureAwait(false);
        }
        return app;
    }

    public static async Task<WebApplication> InitialConnectionJoker(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var gameInterface = scope.ServiceProvider.GetRequiredService<IGameInterfaceJokerClient>();
            _ = await gameInterface
                .CheckingServiceAsync(
                machineName: Environment.MachineName)
                .ConfigureAwait(false);
        }
        return app;
    }

    public static async Task<WebApplication> InitialConnectionOBet(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var gameInterface = scope.ServiceProvider.GetRequiredService<IGameInterfaceOBetClient>();
            _ = await gameInterface
                .CheckingServiceAsync(
                machineName: Environment.MachineName)
                .ConfigureAwait(false);
        }
        return app;
    }

    public static async Task<WebApplication> InitialConnectionPinnacle(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var gameInterface = scope.ServiceProvider.GetRequiredService<IGameInterfacePinnacleClient>();
            _ = await gameInterface
                .CheckingServiceAsync(
                machineName: Environment.MachineName)
                .ConfigureAwait(false);
        }
        return app;
    }

    public static async Task<WebApplication> InitialMemory(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var client = scope.ServiceProvider.GetRequiredService<IGameCommandClient>();

            var gameProviderRepository = scope.ServiceProvider.GetRequiredService<IGameProviderRepository>();

            var gameProviderIdInfos = client
                .GameProviderGetGameProviderIdsAsync()
                .ToEnumerable()
                ?? Enumerable.Empty<GameProviderIdInfo>();

            foreach (var gameProviderIdInfo in gameProviderIdInfos)
            {
                _ = await gameProviderRepository
                    .GetGameProviderIdAsync(
                        gameProviderCode: gameProviderIdInfo.GameProviderCode)
                    .ConfigureAwait(false);

                _ = await gameProviderRepository
                    .GetAsync(
                        gameProviderId: gameProviderIdInfo.GameProviderId)
                    .ConfigureAwait(false);
            }
        }
        return app;
    }
}
