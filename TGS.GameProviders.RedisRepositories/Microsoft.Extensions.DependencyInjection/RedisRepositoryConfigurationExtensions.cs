using System;
using TGS.GameProviders.RedisRepositories;
using TGS.GameProviders.RedisRepositories.Options;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class RedisRepositoryConfigurationExtensions
    {
        public static IServiceCollection AddRedisRepository(this IServiceCollection services,
            Action<RedisOption, IServiceProvider> redisOption)
        {
            services
                .AddOptions<RedisOption>()
                .Configure(redisOption);
            services.AddSingleton<IRedisConnection, RedisConnection>();

            return services
                .AddTransient<IGameProviderScheduleLockRepository, GameProviderScheduleLockRepository>();
        }
    }
}
