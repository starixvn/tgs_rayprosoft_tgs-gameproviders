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
    public static class CoreConfigurationExtensions
    {
        public static IServiceCollection AddCore(this IServiceCollection services)
            => services
            .AddTransient<IKYService, KYService>()
            .AddTransient<IVRService, VRService>()
            .AddTransient<IWGHorseService, WGHorseService>()
            .AddTransient<IWGSportsService, WGSportsService>()
            .AddTransient<IBigGamingService, BigGamingService>()
            .AddTransient<ISexyGamingService, SexyGamingService>()
            .AddTransient<ITCGService, TCGService>();

    }
}
