using System;
using Microsoft.Extensions.DependencyInjection;

namespace TGS.GameProviders.Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection UseRedis(
            this IServiceCollection services,
            Action<RedisOptions, IServiceProvider> configure)
            => services
                .AddOptions<RedisOptions>()
                .Configure(configure)
                .Services
                .AddSingleton<RedisConnectionFactory>();
    }
}
