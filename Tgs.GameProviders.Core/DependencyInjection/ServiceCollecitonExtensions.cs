using System;
using System.Reflection;
using Tgs.GameInterface.AwcService.Client;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.QueryServer.Client;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollecitonExtensions
    {
        public static IServiceCollection AddGameProviders(
            this IServiceCollection services,
            Action<GameProvidersQueryServerClientOptions, IServiceProvider> gameProvidersQueryServerClientOptions,
            Action<GameInterfaceAwcServiceClientOptions, IServiceProvider> gameInterfaceAwcServiceClientOptions)
            => services
                .RegisterAllServiceUtilityTypes(
                    Assembly.GetExecutingAssembly(),
                    ServiceLifetime.Transient)
                .AddGameProvidersQueryServerClient(gameProvidersQueryServerClientOptions)
                .AddGameInterfaceAwcServiceClient(gameInterfaceAwcServiceClientOptions)
                .AddSecurity()//Tgs.Core.Infrastructures.Security
                .AddMemoryCache()
                .AddTransient<IBrandService, BrandService>()
                .AddTransient<IGameActionService, GameActionService>()
                .AddTransient<IGameService, GameService>()
                .AddTransient<IGameProviderService, GameProviderService>()
                .AddTransient<IPlayerService, PlayerService>()
                .AddTransient<IRoundService, RoundService>()
                .AddTransient<ISettingService, SettingService>()
                .AddTransient<ISystemClock, SystemClock>();
    }
}
