using System;
using System.Reflection;
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
using Tgs.GameInterface.WGFmGamingService.Client;
using Tgs.GameInterface.WMCasinoService.Client;
using Tgs.GamePlayerTransform.Client;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.QueryServer.Client;
using Tgs.GameRounds.Client;
using Tgs.GameToken.Client;
using Tgs.GameToken.Clients.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollecitonExtensions
{
    public static IServiceCollection AddGameProviders(
        this IServiceCollection services,
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
        Action<GameInterfaceSabaClientOptions, IServiceProvider> gameInterfaceSabaClientOptions)
        => services
            .RegisterAllServiceUtilityTypes(
                Assembly.GetExecutingAssembly(),
                ServiceLifetime.Transient)
            .AddGameProvidersQueryServerClient(gameProvidersQueryServerClientOptions)
            .AddGameInterfaceAwcServiceClient(gameInterfaceAwcServiceClientOptions)
            .AddGameInterfaceJiliServiceClient(gameInterfaceJiliServiceClientOptions)
            .AddGameInterfaceWMCasinoServiceClient(gameInterfaceWMCasinoClientOptions)
            .AddGameInterfaceEvoClient(gameInterfaceEvolutionServiceClientOptions)
            .AddGameInterfaceIdnLiveServiceClient(gameInterfaceIdnLiveClientOptions)
            .AddGameInterfaceSkywindServiceClient(gameInterfaceSkywindClientOptions)
            .AddGameInterfaceWGFmGamingServiceClient(gameInterfaceWGFmGamingClientOptions)
            .AddGameInterfaceJDBServiceClient(gameInterfaceJDBClientOptions)
            .AddGameInterfaceSBOServiceClient(gameInterfaceSBOClientOptions)
            .AddGameInterfaceBTiServiceClient(gameInterfaceBTiClientOptions)
            .AddGameInterfaceHabaneroServiceClient(gameInterfaceHabaneroClientOptions)
            .AddGameInterfaceSabaServiceClient(gameInterfaceSabaClientOptions)
            .AddGameRoundsClient(gameRoundsClientOptions)
            .AddGameTokenClient(gameTokenClientOptions)
            .AddGamePlayerTransformClient(gamePlayerTransformClientOptions)
            .AddSecurity()//Tgs.Core.Infrastructures.Security
            .AddMemoryCache()
            .AddTransient<IBrandRepository, BrandService>()
            .AddTransient<IGameActionService, GameActionService>()
            .AddTransient<IRoundService, RoundService>()
            .AddTransient<ISystemClock, SystemClock>();
}
