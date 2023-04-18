using Tgs.GameCommand.Client;
using Tgs.GameInterface.AwcService.Client;
using Tgs.GameInterface.BTiService.Client;
using Tgs.GameInterface.DreamGamingService.Client;
using Tgs.GameInterface.EndorphinaService.Client;
using Tgs.GameInterface.EvolutionService.Client;
using Tgs.GameInterface.FaChai.Client;
using Tgs.GameInterface.HabaneroService.Client;
using Tgs.GameInterface.IdnLiveService.Client;
using Tgs.GameInterface.JDBService.Client;
using Tgs.GameInterface.Jili.Client;
using Tgs.GameInterface.OBet.Client;
using Tgs.GameInterface.SabaService.Client;
using Tgs.GameInterface.SBOService.Client;
using Tgs.GameInterface.SkywindService.Client;
using Tgs.GameInterface.SM01Service.Client;
using Tgs.GameInterface.TFGamingService.Client;
using Tgs.GameInterface.WGFmGamingService.Client;
using Tgs.GameInterface.WMCasinoService.Client;
using Tgs.GameInterface.YeeBet.Client;
using Tgs.GamePlayerTransform.Client;
using Tgs.GameProviders;
using Tgs.GameRecord.Endorphina.Session.Client;
using Tgs.GameRounds.Client;
using Tgs.GameToken.Client;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGameProvidersService(
        this IServiceCollection services,
        Action<RedisOptions, IServiceProvider> configure,
        Action<GameRoundsClientOptions, IServiceProvider> gameRoundsClientOptions,
        Action<GameTokenClientOptions, IServiceProvider> gameTokenClientOptions,
        Action<GamePlayerTransformClientOptions, IServiceProvider> gamePlayerTransformClientOptions,
        Action<GameCommandClientOptions, IServiceProvider> gameCommandClientOptions,
        Action<GameInterfaceAwcServiceClientOptions, IServiceProvider> gameInterfaceAwcServiceClientOptions,
        Action<GameInterfaceJiliClientOptions, IServiceProvider> gameInterfaceJiliClientOptions,
        Action<GameInterfaceWMCasinoServiceClientOptions, IServiceProvider> gameInterfaceWMCasinoClientOptions,
        Action<GameInterfaceEvoOptions, IServiceProvider> gameInterfaceEvolutionServiceClientOptions,
        Action<GameInterfaceIdnLiveClientOptions, IServiceProvider> gameInterfaceIdnLiveClientOptions,
        Action<GameInterfaceSkywindClientOptions, IServiceProvider> gameInterfaceSkywindClientOptions,
        Action<GameInterfaceWGFmGamingClientOptions, IServiceProvider> gameInterfaceWGFmGamingClientOptions,
        Action<GameInterfaceJDBClientOptions, IServiceProvider> gameInterfaceJDBClientOptions,
        Action<GameInterfaceSBOClientOptions, IServiceProvider> gameInterfaceSBOClientOptions,
        Action<GameInterfaceBTiClientOptions, IServiceProvider> gameInterfaceBTiClientOptions,
        Action<GameInterfaceHabaneroClientOptions, IServiceProvider> gameInterfaceHabaneroClientOptions,
        Action<GameInterfaceSabaClientOptions, IServiceProvider> gameInterfaceSabaClientOptions,
        Action<GameInterfaceSM01ClientOptions, IServiceProvider> gameInterfaceSM01ClientOptions,
        Action<GameInterfaceTFGamingClientOptions, IServiceProvider> gameInterfaceTFGamingClientOptions,
        Action<GameInterfaceDreamGamingClientOptions, IServiceProvider> gameInterfaceDreamGamingClientOptions,
        Action<GameInterfaceEndorphinaClientOptions, IServiceProvider> gameInterfaceEndorphinaClientOptions,
        Action<GameRecordEndorphinaSessionClientOptions, IServiceProvider> gameRecordEndorphinaSessionClientOptions,
        Action<GameInterfaceFaChaiClientOptions, IServiceProvider> gameInterfaceFaChaiClientOptions,
        Action<GameInterfaceOBetClientOptions, IServiceProvider> gameInterfaceOBetClientOptions,
        Action<GameInterfaceYeeBetClientOptions, IServiceProvider> gameInterfaceYeeBetClientOptions)
        => services
            .AddOptions<RedisOptions>()
            .Configure(configure)
            .Services
            .AddSingleton<RedisConnectionFactory>()
            .AddGameProvidersRepositories()
            .AddGameProvidersServices((sp) => sp.GetRequiredService<RedisConnectionFactory>().Database)
            .AddGameProvidersCore(
                gameRoundsClientOptions: gameRoundsClientOptions,
                gameTokenClientOptions: gameTokenClientOptions,
                gamePlayerTransformClientOptions: gamePlayerTransformClientOptions,
                gameCommandClientOptions: gameCommandClientOptions,
                gameInterfaceAwcServiceClientOptions: gameInterfaceAwcServiceClientOptions,
                gameInterfaceJiliClientOptions: gameInterfaceJiliClientOptions,
                gameInterfaceWMCasinoClientOptions: gameInterfaceWMCasinoClientOptions,
                gameInterfaceEvolutionServiceClientOptions: gameInterfaceEvolutionServiceClientOptions,
                gameInterfaceIdnLiveClientOptions: gameInterfaceIdnLiveClientOptions,
                gameInterfaceSkywindClientOptions: gameInterfaceSkywindClientOptions,
                gameInterfaceWGFmGamingClientOptions: gameInterfaceWGFmGamingClientOptions,
                gameInterfaceJDBClientOptions: gameInterfaceJDBClientOptions,
                gameInterfaceSBOClientOptions: gameInterfaceSBOClientOptions,
                gameInterfaceBTiClientOptions: gameInterfaceBTiClientOptions,
                gameInterfaceHabaneroClientOptions: gameInterfaceHabaneroClientOptions,
                gameInterfaceSabaClientOptions: gameInterfaceSabaClientOptions,
                gameInterfaceSM01ClientOptions: gameInterfaceSM01ClientOptions,
                gameInterfaceTFGamingClientOptions: gameInterfaceTFGamingClientOptions,
                gameInterfaceDreamGamingClientOptions: gameInterfaceDreamGamingClientOptions,
                gameInterfaceEndorphinaClientOptions: gameInterfaceEndorphinaClientOptions,
                gameRecordEndorphinaClientOptions: gameRecordEndorphinaSessionClientOptions,
                gameInterfaceFaChaiClientOptions: gameInterfaceFaChaiClientOptions,
                gameInterfaceOBetClientOptions: gameInterfaceOBetClientOptions,
                gameInterfaceYeeBetClientOptions: gameInterfaceYeeBetClientOptions);
}
