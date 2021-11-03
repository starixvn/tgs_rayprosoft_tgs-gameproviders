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
using TGS.GameProviders.Services.BigGaming;
using TGS.GameProviders.Services.Evolution;
using TGS.GameProviders.Services.KY;
using TGS.GameProviders.Services.LY;
using TGS.GameProviders.Services.SBO;
using TGS.GameProviders.Services.SexyGaming;
using TGS.GameProviders.Services.Skywind;
using TGS.GameProviders.Services.TCG;
using TGS.GameProviders.Services.VR;
using TGS.GameProviders.Services.WGHorse;
using TGS.GameProviders.Services.WGSports;
using TGS.GameProviders.Services.WMCasino;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class GameProvidersServicesConfigurationExtensions
    {
        public static IServiceCollection AddGameProvidersServices(this IServiceCollection services)
            => services
            .AddTransient<IKYService, KYService>()
            .AddTransient<IVRService, VRService>()
            .AddTransient<IVRGamePlayer, VRGamePlayer>()
            .AddTransient<IWGHorseService, WGHorseService>()
            .AddTransient<IWGHorseGamePlayer, WGHorseGamePlayer>()
            .AddTransient<IWGSportsService, WGSportsService>()
            .AddTransient<IWGSportsGamePlayer, WGSportsGamePlayer>()
            .AddTransient<IBigGamingService, BigGamingService>()
            .AddTransient<IBigGamingGamePlayer, BigGamingGamePlayer>()
            .AddTransient<IBigGamingGameUrl, BigGamingGameUrl>()
            .AddTransient<ISexyGamingService, SexyGamingService>()
            .AddTransient<ISexyGamingGamePlayer, SexyGamingGamePlayer>()
            .AddTransient<ISkywindService, SkywindService>()
            .AddTransient<ISkywindAccessToken, SkywindAccessToken>()
            .AddTransient<ITCGService, TCGService>()
            .AddTransient<ITCGGamePlayer, TCGGamePlayer>()
            .AddTransient<IEvolutionService, EvolutionService>()
            .AddTransient<ILYService, LYService>()
            .AddTransient<ISBOService, SBOService>()
            .AddTransient<ISBOGamePlayer, SBOGamePlayer>()
            .AddTransient<IWMCasinoService, WMCasinoService>()
            .AddTransient<IWMCasinoAgentMapper, WMCasinoAgentMapper>()
            .AddTransient<IWMCasinoUtility, WMCasinoUtility>()
            .AddTransient<IWMCasinoGamePlayer, WMCasinoGamePlayer>();
    }
}
