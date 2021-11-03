using System;
using Microsoft.EntityFrameworkCore;
using TGS.GameProviders.Core.Repositories;
using TGS.GameProviders.Repositories;
using TGS.GameProviders.Repositories.Entities;
using TGS.GameProviders.Repositories.ModelMappings;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class RepositoryConfigurationExtensions
    {
        public static IServiceCollection AddDbBetRepository(this IServiceCollection services,
            Action<IServiceProvider, DbContextOptionsBuilder> efConfigure)
        {
            services.AddDbContext<BetContext>(efConfigure);
            services.AddAutoMapper(config =>
            {
                config.AddProfile<RoundMapping>();
            });
            return services
                .AddTransient<IRoundRepository, RoundRepository>();
        }

        public static IServiceCollection AddDbTgsRepository(this IServiceCollection services,
                    Action<IServiceProvider, DbContextOptionsBuilder> efConfigure)
        {
            services.AddDbContext<TgsContext>(efConfigure);
            services.AddAutoMapper(config =>
            {
                config.AddProfile<GameMapping>();
                config.AddProfile<PlayerMapping>();
            });
            return services
                .AddTransient<IGameProviderRepository, GameProviderRepository>()
                .AddTransient<ISettingRepository, SettingRepository>()
                .AddTransient<IGameRepository, GameRepository>()
                .AddTransient<IBrandRepository, BrandRepository>()
                .AddTransient<IPlayerRepository, PlayerRepository>();
        }
    }
}
