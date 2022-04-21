using System;
using Tgs.GameProviders;

namespace Microsoft.Extensions.DependencyInjection
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
