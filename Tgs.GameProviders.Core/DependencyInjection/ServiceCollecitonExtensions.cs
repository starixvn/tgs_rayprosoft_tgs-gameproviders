using System;
using System.Reflection;
using Tgs.GameInterface.AwcService.Client;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Core.AwcVenus;
using Tgs.GameProviders.QueryServer.Client;
using Tgs.GameRounds.Client;
using Tgs.GameProviders.Core.AwcSpade;
using Tgs.GameProviders.Core.AwcYl;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollecitonExtensions
    {
        public static IServiceCollection AddGameProviders(
            this IServiceCollection services,
            Action<GameProvidersQueryServerClientOptions, IServiceProvider> gameProvidersQueryServerClientOptions,
            Action<GameInterfaceAwcServiceClientOptions, IServiceProvider> gameInterfaceAwcServiceClientOptions,
            Action<GameRoundsClientOptions, IServiceProvider> gameRoundsClientOptions)
            => services
                .RegisterAllServiceUtilityTypes(
                    Assembly.GetExecutingAssembly(),
                    ServiceLifetime.Transient)
                .AddGameProvidersQueryServerClient(gameProvidersQueryServerClientOptions)
                .AddGameInterfaceAwcServiceClient(gameInterfaceAwcServiceClientOptions)
                .AddGameRoundsClient(gameRoundsClientOptions)
                .AddSecurity()//Tgs.Core.Infrastructures.Security
                .AddMemoryCache()
                .AddTransient<IBrandService, BrandService>()
                .AddTransient<IGameActionService, GameActionService>()
                .AddTransient<IGameService, GameService>()
                .AddTransient<IRoundService, RoundService>()
                .AddTransient<ISystemClock, SystemClock>()
                .AddScoped<IAwcSpadeGamePlayer, AwcSpadeGamePlayer>()
                .AddScoped<IAwcVenusGamePlayer, AwcVenusGamePlayer>()
                .AddScoped<IAwcYlGamePlayer, AwcYlGamePlayer>();
    }
}
