using Tgs.GameProviders.Core;
using Tgs.GameProviders.Repositories;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGameProvidersRepositories(this IServiceCollection services)
   => services
        .Scan(scan =>
        {
            _ = scan.FromCallingAssembly()
            .AddClasses(classes => classes.Where(t => t.Name.EndsWith("repository", StringComparison.OrdinalIgnoreCase)))
            .AsImplementedInterfaces()
            .WithTransientLifetime();
        })
        .AddTransient<ISettingRepository, SettingRepository>()
        .Decorate<ISettingRepository>((innerRepository, sp) => ActivatorUtilities.CreateInstance<SettingCacheRepository>(sp, innerRepository))
        .AddTransient<IGameProviderRepository, GameProviderRepository>()
        .Decorate<IGameProviderRepository>((innerRepository, sp) => ActivatorUtilities.CreateInstance<GameProviderCacheRepository>(sp, innerRepository))
        .AddTransient<IPlayerRepository, PlayerRepository>()
        .Decorate<IPlayerRepository>((innerRepository, sp) => ActivatorUtilities.CreateInstance<PlayerCacheRepository>(sp, innerRepository))
        .AddTransient<IGameRepository, GameRepository>()
        .Decorate<IGameRepository>((innerRepository, sp) => ActivatorUtilities.CreateInstance<GameCacheRepository>(sp, innerRepository));
}
