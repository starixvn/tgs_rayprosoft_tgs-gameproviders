using System;
using System.Reflection;
using Tgs.GameInterface.AwcService.Client;
using Tgs.GameInterface.JiliService.Client;
using Tgs.GameInterface.WMCasinoService.Client;
using Tgs.GameInterface.EvolutionService.Client;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.QueryServer.Client;
using Tgs.GameRounds.Client;
using Tgs.GameInterface.IdnLiveService.Client;
using Tgs.GameToken.Client;
using Tgs.GameToken.Clients.DependencyInjection;
using Tgs.GamePlayerTransform.Client;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollecitonExtensions
    {
        public static IServiceCollection AddGameProviders(
            this IServiceCollection services,
            Action<GameProvidersQueryServerClientOptions, IServiceProvider> gameProvidersQueryServerClientOptions,
            Action<GameInterfaceAwcServiceClientOptions, IServiceProvider> gameInterfaceAwcServiceClientOptions,
            Action<GameInterfaceJiliServiceClientOptions, IServiceProvider> gameInterfaceJiliServiceClientOptions,
            Action<GameInterfaceWMCasinoServiceClientOptions, IServiceProvider> gameInterfaceWMCasinoClientOptions,
            Action<GameInterfaceEvoOptions, IServiceProvider> gameInterfaceEvolutionServiceClientOptions,
            Action<GameInterfaceIdnLiveClientOptions, IServiceProvider> gameInterfaceIdnLiveClientOptions,
            Action<GameRoundsClientOptions, IServiceProvider> gameRoundsClientOptions,
            Action<GameTokenClientOptions, IServiceProvider> gameTokenClientOptions,
            Action<GamePlayerTransformClientOptions, IServiceProvider> gamePlayerTransformClientOptions)
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
                .AddGameRoundsClient(gameRoundsClientOptions)
                .AddGameTokenClient(gameTokenClientOptions)
                .AddGamePlayerTransformClient(gamePlayerTransformClientOptions)
                .AddSecurity()//Tgs.Core.Infrastructures.Security
                .AddMemoryCache()
                .AddTransient<IBrandService, BrandService>()
                .AddTransient<IGameActionService, GameActionService>()
                .AddTransient<IRoundService, RoundService>()
                .AddTransient<ISystemClock, SystemClock>();
    }
}