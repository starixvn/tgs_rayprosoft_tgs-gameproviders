using Tgs.GameCommand.Client;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.AVIAGaming.Client;
using Tgs.GameInterface.AwcService.Client;
using Tgs.GameInterface.BTiService.Client;
using Tgs.GameInterface.Cards365.Client;
using Tgs.GameInterface.DreamGamingService.Client;
using Tgs.GameInterface.EndorphinaService.Client;
using Tgs.GameInterface.EvolutionService.Client;
using Tgs.GameInterface.FaChai.Client;
using Tgs.GameInterface.HabaneroService.Client;
using Tgs.GameInterface.InplayMatrix.Client;
using Tgs.GameInterface.JDBService.Client;
using Tgs.GameInterface.Jili.Client;
using Tgs.GameInterface.Joker.Client;
using Tgs.GameInterface.Kaiyuan.Client;
using Tgs.GameInterface.LEGaming.Client;
using Tgs.GameInterface.OBet.Client;
using Tgs.GameInterface.Pinnacle.Client;
using Tgs.GameInterface.PragmaticPlay.Client;
using Tgs.GameInterface.SabaService.Client;
using Tgs.GameInterface.SBOService.Client;
using Tgs.GameInterface.SkywindService.Client;
using Tgs.GameInterface.SM01Service.Client;
using Tgs.GameInterface.TFGamingService.Client;
using Tgs.GameInterface.WGFmGamingService.Client;
using Tgs.GameInterface.WGViaGaming.Client;
using Tgs.GameInterface.WicketCricket.Client;
using Tgs.GameInterface.WMCasinoService.Client;
using Tgs.GameInterface.YeeBet.Client;
using Tgs.GameProviders.Core;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceRegisterExtensions
{
    public static async Task<WebApplication> InitialConnectionAVIAGaming(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var gameInterface = scope.ServiceProvider.GetRequiredService<IGameInterfaceAVIAGamingClient>();
            _ = await gameInterface
                .CheckingServiceAsync(
                machineName: Environment.MachineName)
                .ConfigureAwait(false);
        }
        return app;
    }

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

    public static async Task<WebApplication> InitialConnectionEndorphina(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var gameInterface = scope.ServiceProvider.GetRequiredService<IGameInterfaceEndorphinaClient>();
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

    public static async Task<WebApplication> InitialConnectionHabanero(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var gameInterface = scope.ServiceProvider.GetRequiredService<IGameInterfaceHabaneroClient>();
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

    public static async Task<WebApplication> InitialConnectionJDB(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var gameInterface = scope.ServiceProvider.GetRequiredService<IGameInterfaceJDBClient>();
            _ = await gameInterface
                .CheckingServiceAsync(
                machineName: Environment.MachineName)
                .ConfigureAwait(false);
        }
        return app;
    }

    public static async Task<WebApplication> InitialConnectionJili(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var gameInterface = scope.ServiceProvider.GetRequiredService<IGameInterfaceJiliClient>();
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

    public static async Task<WebApplication> InitialConnectionKaiyuan(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var gameInterface = scope.ServiceProvider.GetRequiredService<IGameInterfaceKaiyuanClient>();
            _ = await gameInterface
                .CheckingServiceAsync(
                machineName: Environment.MachineName)
                .ConfigureAwait(false);
        }
        return app;
    }

    public static async Task<WebApplication> InitialConnectionLEGaming(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var gameInterface = scope.ServiceProvider.GetRequiredService<IGameInterfaceLEGamingClient>();
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

    public static async Task<WebApplication> InitialConnectionPragmaticPlay(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var gameInterface = scope.ServiceProvider.GetRequiredService<IGameInterfacePragmaticPlayClient>();
            _ = await gameInterface
                .CheckingServiceAsync(
                machineName: Environment.MachineName)
                .ConfigureAwait(false);
        }
        return app;
    }

    public static async Task<WebApplication> InitialConnectionSaba(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var gameInterface = scope.ServiceProvider.GetRequiredService<IGameInterfaceSabaClient>();
            _ = await gameInterface
                .CheckingServiceAsync(
                machineName: Environment.MachineName)
                .ConfigureAwait(false);
        }
        return app;
    }

    public static async Task<WebApplication> InitialConnectionSBO(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var gameInterface = scope.ServiceProvider.GetRequiredService<IGameInterfaceSBOClient>();
            _ = await gameInterface
                .CheckingServiceAsync(
                machineName: Environment.MachineName)
                .ConfigureAwait(false);
        }
        return app;
    }

    public static async Task<WebApplication> InitialConnectionSkywind(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var gameInterface = scope.ServiceProvider.GetRequiredService<IGameInterfaceSkywindClient>();
            _ = await gameInterface
                .CheckingServiceAsync(
                machineName: Environment.MachineName)
                .ConfigureAwait(false);
        }
        return app;
    }

    public static async Task<WebApplication> InitialConnectionSM01(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var gameInterface = scope.ServiceProvider.GetRequiredService<IGameInterfaceSM01Client>();
            _ = await gameInterface
                .CheckingServiceAsync(
                machineName: Environment.MachineName)
                .ConfigureAwait(false);
        }
        return app;
    }

    public static async Task<WebApplication> InitialConnectionTFGaming(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var gameInterface = scope.ServiceProvider.GetRequiredService<IGameInterfaceTFGamingClient>();
            _ = await gameInterface
                .CheckingServiceAsync(
                machineName: Environment.MachineName)
                .ConfigureAwait(false);
        }
        return app;
    }

    public static async Task<WebApplication> InitialConnectionWGFmGaming(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var gameInterface = scope.ServiceProvider.GetRequiredService<IGameInterfaceWGFmGamingClient>();
            _ = await gameInterface
                .CheckingServiceAsync(
                machineName: Environment.MachineName)
                .ConfigureAwait(false);
        }
        return app;
    }

    public static async Task<WebApplication> InitialConnectionWGViaGaming(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var gameInterface = scope.ServiceProvider.GetRequiredService<IGameInterfaceWGViaGamingClient>();
            _ = await gameInterface
                .CheckingServiceAsync(
                machineName: Environment.MachineName)
                .ConfigureAwait(false);
        }
        return app;
    }

    public static async Task<WebApplication> InitialConnectionWMCasino(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var gameInterface = scope.ServiceProvider.GetRequiredService<IGameInterfaceWMCasinoServiceClient>();
            _ = await gameInterface
                .CheckingServiceAsync(
                machineName: Environment.MachineName)
                .ConfigureAwait(false);
        }
        return app;
    }

    public static async Task<WebApplication> InitialConnectionWicketCricke(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var gameInterface = scope.ServiceProvider.GetRequiredService<IGameInterfaceWicketCricketClient>();
            _ = await gameInterface
                .CheckingServiceAsync(
                machineName: Environment.MachineName)
                .ConfigureAwait(false);
        }
        return app;
    }

    
    public static async Task<WebApplication> InitialConnectionYeeBet(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var gameInterface = scope.ServiceProvider.GetRequiredService<IGameInterfaceYeeBetClient>();
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

            var gameProviderInfos = client
                .GameProviderQueryAsync()
                .ToEnumerable()
                ?? Enumerable.Empty<GameProviderInfo>();

            foreach (var gameProviderInfo in gameProviderInfos)
            {
                _ = await gameProviderRepository
                    .GetGameProviderIdAsync(
                        gameProviderCode: gameProviderInfo.GameProviderCode)
                    .ConfigureAwait(false);

                _ = await gameProviderRepository
                    .GetAsync(
                        gameProviderId: gameProviderInfo.GameProviderId)
                    .ConfigureAwait(false);
            }
        }
        return app;
    }
}
