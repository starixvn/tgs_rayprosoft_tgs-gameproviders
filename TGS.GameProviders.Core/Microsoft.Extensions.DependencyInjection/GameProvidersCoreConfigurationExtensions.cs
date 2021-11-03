// ReSharper disable CheckNamespace

using System.Reflection;
using TGS.GameProviders.Core;
using TGS.GameProviders.Core.BigGaming;
using TGS.GameProviders.Core.Evolution;
using TGS.GameProviders.Core.KY;
using TGS.GameProviders.Core.LY;
using TGS.GameProviders.Core.SBO;
using TGS.GameProviders.Core.SexyGaming;
using TGS.GameProviders.Core.Skywind;
using TGS.GameProviders.Core.TCG;
using TGS.GameProviders.Core.VR;
using TGS.GameProviders.Core.WGHorse;
using TGS.GameProviders.Core.WGSports;
using TGS.GameProviders.Core.WMCasino;

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
                .AddTransient<IVRGamePlayer, VRGamePlayer>();

            services
                .AddTransient<IWGHorseService, WGHorseService>()
                .AddTransient<IWGHorseGamePlayer, WGHorseGamePlayer>();

            services
                .AddTransient<IWGSportsService, WGSportsService>()
                .AddTransient<IWGSportsGamePlayer, WGSportsGamePlayer>();

            services
                .AddTransient<IBigGamingService, BigGamingService>()
                .AddTransient<IBigGamingGamePlayer, BigGamingGamePlayer>()
                .AddTransient<IBigGamingGameUrl, BigGamingGameUrl>();

            services
                .AddTransient<ISexyGamingService, SexyGamingService>()
                .AddTransient<ISexyGamingGamePlayer, SexyGamingGamePlayer>();

            services
                .AddTransient<ISkywindService, SkywindService>()
                .AddTransient<ISkywindAccessToken, SkywindAccessToken>();

            services
                .AddTransient<ITCGService, TCGService>()
                .AddTransient<ITCGGamePlayer, TCGGamePlayer>();

            services
                .AddTransient<IEvolutionService, EvolutionService>();

            services
                .AddTransient<ILYService, LYService>()
                .AddTransient<ILYGameProviderScheduleService, LYGameProviderScheduleService>();

            services
                .AddTransient<ISBOService, SBOService>()
                .AddTransient<ISBOUtility, SBOUtility>()
                .AddTransient<ISBOGamePlayer, SBOGamePlayer>()
                .AddTransient<ISBOGameAgent, SBOGameAgent>();

            services
                .AddTransient<IWMCasinoService, WMCasinoService>()
                .AddTransient<IWMCasinoAgentMapper, WMCasinoAgentMapper>()
                .AddTransient<IWMCasinoUtility, WMCasinoUtility>()
                .AddTransient<IWMCasinoGamePlayer, WMCasinoGamePlayer>();

            return services;
        }
    }
}