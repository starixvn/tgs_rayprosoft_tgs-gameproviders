using System;
using Tgs.GameInterface.AwcService.Client;
using Tgs.GameInterface.EvolutionService.Client;
using Tgs.GameInterface.IdnLiveService.Client;
using Tgs.GameInterface.JiliService.Client;
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
            .AddOptions<RedisOptions>()
            .Configure(configure)
            .Services
            .AddSingleton<RedisConnectionFactory>()
            .AddGameProviders(
                gameProvidersQueryServerClientOptions,
                gameInterfaceAwcServiceClientOptions,
                gameInterfaceJiliServiceClientOptions,
                gameInterfaceWMCasinoClientOptions,
                gameInterfaceEvolutionServiceClientOptions,
                gameInterfaceIdnLiveClientOptions,
                gameRoundsClientOptions,
                gameTokenClientOptions,
                gamePlayerTransformClientOptions);
}
