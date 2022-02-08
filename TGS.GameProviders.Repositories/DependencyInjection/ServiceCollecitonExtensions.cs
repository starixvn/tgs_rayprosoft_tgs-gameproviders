using System;
using Microsoft.EntityFrameworkCore;
using TGS.GameProviders.Core.Repositories;
using TGS.GameProviders.Repositories;
using TGS.GameProviders.Repositories.Entities;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollecitonExtensions
    {
        public static IServiceCollection AddDbTgsRepository(this IServiceCollection services,
                    Action<IServiceProvider, DbContextOptionsBuilder> efConfigure)
        {
            services.AddDbContext<TgsContext>(efConfigure);

            return services
                .AddTransient<IGameProviderRepository, GameProviderRepository>();
        }
    }
}
