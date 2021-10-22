using System.Reflection;
using TGS.GameProviders.Core;
using TGS.GameProviders.Core.BigGaming;
using TGS.GameProviders.Core.KY;
using TGS.GameProviders.Core.SexyGaming;
using TGS.GameProviders.Core.Skywind;
using TGS.GameProviders.Core.TCG;
using TGS.GameProviders.Core.VR;
using TGS.GameProviders.Core.WGHorse;
using TGS.GameProviders.Core.WGSports;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class GameProvidersCoreConfigurationExtensions
    {
        public static IServiceCollection AddGameProvidersCore(this IServiceCollection services)
        {
            services
                .RegisterAllServiceUtilityTypes(
                Assembly.GetExecutingAssembly(),
                ServiceLifetime.Transient);

            //Tgs.Core.Infrastructures.Security
            services.AddSecurity();

            services
               .AddTransient<IKYService, KYService>();

            services
             .AddTransient<IVRService, VRService>()
             .AddTransient<IVRGamePlayerCreate, VRGamePlayerCreate>();

            services
                .AddTransient<IWGHorseService, WGHorseService>()
                .AddTransient<IWGHorseGamePlayerCreate, WGHorseGamePlayerCreate>();

            services
                .AddTransient<IWGSportsService, WGSportsService>()
                .AddTransient<IWGSportsGamePlayerCreate, WGSportsGamePlayerCreate>();

            services
                .AddTransient<IBigGamingService, BigGamingService>();

            services
                 .AddTransient<ISexyGamingService, SexyGamingService>();

            services
                .AddTransient<ISkywindService, SkywindService>()
                .AddTransient<ISkywindAccessToken, SkywindAccessToken>();

            services
                .AddTransient<ITCGService, TCGService>()
                .AddTransient<ITCGGamePlayerCreate, TCGGamePlayerCreate>();

            return services;
        }
    }
}
