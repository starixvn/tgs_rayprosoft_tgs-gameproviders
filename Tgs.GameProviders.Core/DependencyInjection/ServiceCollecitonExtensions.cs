using System;
using System.Reflection;
using Tgs.GameInterface.AwcService.Client;
using Tgs.GameInterface.JiliService.Client;
using Tgs.GameInterface.WMCasinoService.Client;
using Tgs.GameInterface.EvolutionService.Client;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.QueryServer.Client;
using Tgs.GameRounds.Client;

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
            Action<GameRoundsClientOptions, IServiceProvider> gameRoundsClientOptions)
            => services
                .RegisterAllServiceUtilityTypes(
                    Assembly.GetExecutingAssembly(),
                    ServiceLifetime.Transient)
                .AddGameProvidersQueryServerClient(gameProvidersQueryServerClientOptions)
                .AddGameInterfaceAwcServiceClient(gameInterfaceAwcServiceClientOptions)
                .AddGameInterfaceJiliServiceClient(gameInterfaceJiliServiceClientOptions)
                .AddGameInterfaceWMCasinoServiceClient(gameInterfaceWMCasinoClientOptions)
                .AddGameInterfaceEvoClient(gameInterfaceEvolutionServiceClientOptions)
                .AddGameRoundsClient(gameRoundsClientOptions)
                .AddSecurity()//Tgs.Core.Infrastructures.Security
                .AddMemoryCache()
                .AddTransient<IBrandService, BrandService>()
                .AddTransient<IGameActionService, GameActionService>()
                .AddTransient<IRoundService, RoundService>()
                .AddTransient<ISystemClock, SystemClock>();
    }
}