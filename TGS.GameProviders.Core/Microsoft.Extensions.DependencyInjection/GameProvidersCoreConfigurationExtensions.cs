using System.Reflection;
using TGS.GameProviders.Core;
using TGS.GameProviders.Core.BigGaming;
using TGS.GameProviders.Core.KY;
using TGS.GameProviders.Core.SexyGaming;
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
            return services
               .AddTransient<IKYService, KYService>()
               .AddTransient<IVRService, VRService>()
               .AddTransient<IVRGamePlayerCreate, VRGamePlayerCreate>()
               .AddTransient<IWGHorseService, WGHorseService>()
               .AddTransient<IWGHorseGamePlayerCreate, WGHorseGamePlayerCreate>()
               .AddTransient<IWGSportsService, WGSportsService>()
               .AddTransient<IWGSportsGamePlayerCreate, WGSportsGamePlayerCreate>()
               .AddTransient<IBigGamingService, BigGamingService>()
               .AddTransient<ISexyGamingService, SexyGamingService>()
               .AddTransient<ITCGService, TCGService>();
        }
    }
}
