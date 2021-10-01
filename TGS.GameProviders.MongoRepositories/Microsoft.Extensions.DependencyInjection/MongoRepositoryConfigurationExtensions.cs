using System;
using TGS.GameProviders.MongoRepositories;
using TGS.GameProviders.MongoRepositories.Options;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MongoRepositoryConfigurationExtensions
    {
        public static IServiceCollection AddMongoRepository(this IServiceCollection services,
            Action<MongoDBOption, IServiceProvider> mongoDBOption)
        {
            services
                .AddOptions<MongoDBOption>()
                .Configure(mongoDBOption);

            return services
                .AddTransient<IGameProviderScheduleRepository, GameProviderScheduleRepository>();
        }
    }
}
