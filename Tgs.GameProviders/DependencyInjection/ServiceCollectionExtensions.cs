using Tgs.GameInterface.AwcService.Client;
using Tgs.GameInterface.BTiService.Client;
using Tgs.GameInterface.EvolutionService.Client;
using Tgs.GameInterface.HabaneroService.Client;
using Tgs.GameInterface.IdnLiveService.Client;
using Tgs.GameInterface.JDBService.Client;
using Tgs.GameInterface.JiliService.Client;
using Tgs.GameInterface.SabaService.Client;
using Tgs.GameInterface.SBOService.Client;
using Tgs.GameInterface.SkywindService.Client;
using Tgs.GameInterface.SM01Service.Client;
using Tgs.GameInterface.TFGamingService.Client;
using Tgs.GameInterface.WGFmGamingService.Client;
using Tgs.GameInterface.WMCasinoService.Client;
using Tgs.GamePlayerTransform.Client;
using Tgs.GameProviders;
using Tgs.GameProviders.QueryServer.Client;
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
        Action<GameProvidersQueryServerClientOptions, IServiceProvider> gameProvidersQueryServerClientOptions,
        Action<GameInterfaceAwcServiceClientOptions, IServiceProvider> gameInterfaceAwcServiceClientOptions,
        Action<GameInterfaceJiliServiceClientOptions, IServiceProvider> gameInterfaceJiliServiceClientOptions,
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
        Action<GameInterfaceTFGamingClientOptions, IServiceProvider> gameInterfaceTFGamingClientOptions)
        => services
            .AddOptions<RedisOptions>()
            .Configure(configure)
            .Services
            .AddSingleton<RedisConnectionFactory>()
            .AddGameProviders(
                gameRoundsClientOptions,
                gameTokenClientOptions,
                gamePlayerTransformClientOptions,
                gameProvidersQueryServerClientOptions,
                gameInterfaceAwcServiceClientOptions,
                gameInterfaceJiliServiceClientOptions,
                gameInterfaceWMCasinoClientOptions,
                gameInterfaceEvolutionServiceClientOptions,
                gameInterfaceIdnLiveClientOptions,
                gameInterfaceSkywindClientOptions,
                gameInterfaceWGFmGamingClientOptions,
                gameInterfaceJDBClientOptions,
                gameInterfaceSBOClientOptions,
                gameInterfaceBTiClientOptions,
                gameInterfaceHabaneroClientOptions,
                gameInterfaceSabaClientOptions,
                gameInterfaceSM01ClientOptions,
                gameInterfaceTFGamingClientOptions);
}
