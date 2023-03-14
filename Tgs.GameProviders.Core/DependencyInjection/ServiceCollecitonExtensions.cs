using System;
using System.Reflection;
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
using Tgs.GameInterface.JiliService.Client;
using Tgs.GameInterface.SabaService.Client;
using Tgs.GameInterface.SBOService.Client;
using Tgs.GameInterface.SkywindService.Client;
using Tgs.GameInterface.SM01Service.Client;
using Tgs.GameInterface.TFGamingService.Client;
using Tgs.GameInterface.WGFmGamingService.Client;
using Tgs.GameInterface.WMCasinoService.Client;
using Tgs.GamePlayerTransform.Client;
using Tgs.GameProviders.Core;
using Tgs.GameRecord.Endorphina.Session.Client;
using Tgs.GameRounds.Client;
using Tgs.GameToken.Client;
using Tgs.GameToken.Clients.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollecitonExtensions
{
    public static IServiceCollection AddGameProvidersCore(
        this IServiceCollection services,
        Action<GameRoundsClientOptions, IServiceProvider> gameRoundsClientOptions,
        Action<GameTokenClientOptions, IServiceProvider> gameTokenClientOptions,
        Action<GamePlayerTransformClientOptions, IServiceProvider> gamePlayerTransformClientOptions,
        Action<GameCommandClientOptions, IServiceProvider> gameCommandClientOptions,
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
        Action<GameInterfaceTFGamingClientOptions, IServiceProvider> gameInterfaceTFGamingClientOptions,
        Action<GameInterfaceDreamGamingClientOptions, IServiceProvider> gameInterfaceDreamGamingClientOptions,
        Action<GameInterfaceEndorphinaClientOptions, IServiceProvider> gameInterfaceEndorphinaClientOptions,
        Action<GameRecordEndorphinaSessionClientOptions, IServiceProvider> gameRecordEndorphinaClientOptions,
        Action<GameInterfaceFaChaiClientOptions, IServiceProvider> gameInterfaceFaChaiClientOptions)
        => services
            .RegisterAllServiceUtilityTypes(
                Assembly.GetExecutingAssembly(),
                ServiceLifetime.Transient)
            .AddGameCommandClient(gameCommandClientOptions)
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
            .AddGameInterfaceSM01ServiceClient(gameInterfaceSM01ClientOptions)
            .AddGameInterfaceTFGamingServiceClient(gameInterfaceTFGamingClientOptions)
            .AddGameInterfaceDreamGamingServiceClient(gameInterfaceDreamGamingClientOptions)
            .AddGameInterfaceEndorphinaServiceClient(gameInterfaceEndorphinaClientOptions)
            .AddGameInterfaceFaChaiClient(gameInterfaceFaChaiClientOptions)
            .AddGameRecordEndorphinaSessionClient(gameRecordEndorphinaClientOptions)
            .AddGameRoundsClient(gameRoundsClientOptions)
            .AddGameTokenClient(gameTokenClientOptions)
            .AddGamePlayerTransformClient(gamePlayerTransformClientOptions)
            .AddSecurity()//Tgs.Core.Infrastructures.Security
            .AddMemoryCache()
            .AddTransient<IGameActionService, GameActionService>()
            .AddTransient<IRoundService, RoundService>()
            .AddTransient<ISystemClock, SystemClock>();
}
