using Tgs.GameCommand.Client;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.Awc.Client;
using Tgs.GameInterface.BTiService.Client;
using Tgs.GameInterface.Cards365.Client;
using Tgs.GameInterface.CrPati.Client;
using Tgs.GameInterface.DreamGamingService.Client;
using Tgs.GameInterface.EndorphinaService.Client;
using Tgs.GameInterface.EvolutionService.Client;
using Tgs.GameInterface.FaChai.Client;
using Tgs.GameInterface.GKXGaming.Client;
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
using Tgs.GameInterface.TFGamingService.Client;
using Tgs.GameInterface.V8Poker.Client;
using Tgs.GameInterface.WGFmGamingService.Client;
using Tgs.GameInterface.WGViaGaming.Client;
using Tgs.GameInterface.WicketCricket.Client;
using Tgs.GameInterface.WMCasinoService.Client;
using Tgs.GameInterface.YeeBet.Client;
using Tgs.GameProviders.Core;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceRegisterExtensions
{

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

    public static WebApplication InitialSnowflakeWork(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var snowflakeWork = scope.ServiceProvider.GetRequiredService<ISnowflakeWork>();

            _ = snowflakeWork.GetId();
        }
        return app;
    }
}
