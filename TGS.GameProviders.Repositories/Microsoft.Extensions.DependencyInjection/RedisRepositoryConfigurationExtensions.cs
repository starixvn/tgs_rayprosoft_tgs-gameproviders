using System;
using TGS.GameProviders.Repositories;
using TGS.GameProviders.Repositories.Options;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class RedisRepositoryConfigurationExtensions
    {
        public static IServiceCollection AddRedisRepository(this IServiceCollection services,
            Action<RedisOptions, IServiceProvider> redisOptions)
            =>
            services
            .AddOptions<RedisOptions>()
            .Configure(redisOptions)
            .Services
            .AddSingleton<RedisConnectionFactory>();
    }
}
