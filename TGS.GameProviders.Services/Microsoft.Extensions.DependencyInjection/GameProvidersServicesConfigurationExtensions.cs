using System;
using StackExchange.Redis;
using TGS.GameProviders.Core.AsiaGaming;
using TGS.GameProviders.Core.BigGaming;
using TGS.GameProviders.Core.Evolution;
using TGS.GameProviders.Core.IMESports;
using TGS.GameProviders.Core.KY;
using TGS.GameProviders.Core.LY;
using TGS.GameProviders.Core.PragmaticPlay;
using TGS.GameProviders.Core.Saba;
using TGS.GameProviders.Core.SBO;
using TGS.GameProviders.Core.SexyGaming;
using TGS.GameProviders.Core.Skywind;
using TGS.GameProviders.Core.TCG;
using TGS.GameProviders.Core.VR;
using TGS.GameProviders.Core.WGHorse;
using TGS.GameProviders.Core.WGSports;
using TGS.GameProviders.Core.WMCasino;
using TGS.GameProviders.Services.AsiaGaming;
using TGS.GameProviders.Services.BigGaming;
using TGS.GameProviders.Services.Evolution;
using TGS.GameProviders.Services.IMESports;
using TGS.GameProviders.Services.KY;
using TGS.GameProviders.Services.LY;
using TGS.GameProviders.Services.PragmaticPlay;
using TGS.GameProviders.Services.Saba;
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
        public static IServiceCollection AddGameProvidersServices(
            this IServiceCollection services,
             Func<IServiceProvider, IDatabase> redisDatabaseResolver)
        {
            services
                .AddTransient<IKYService, KYService>();

            services
                .AddTransient<IVRService, VRService>();

            services
                .AddTransient<IVRGamePlayer, VRGamePlayer>();

            services
                .AddTransient<IWGHorseService, WGHorseService>()
                .AddTransient<IWGHorseGamePlayer, WGHorseGamePlayer>()
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
                .AddTransient<ISkywindService, SkywindService>();
            services
                .AddTransient<ISkywindAccessToken>(sp => ActivatorUtilities.CreateInstance<SkywindAccessToken>(sp, redisDatabaseResolver(sp)));

            services
                .AddTransient<ITCGService, TCGService>()
                .AddTransient<ITCGGamePlayer, TCGGamePlayer>();

            services
                .AddTransient<IEvolutionService, EvolutionService>();

            services
                .AddTransient<ILYService, LYService>();

            services
                .AddTransient<ISBOService, SBOService>()
                .AddTransient<ISBOGamePlayer, SBOGamePlayer>()
                .AddTransient<ISBOUtility, SBOUtility>()
                .AddTransient<ISBOGameAgent, SBOGameAgent>();

            services
                .AddTransient<IWMCasinoService, WMCasinoService>()
                .AddTransient<IWMCasinoAgentMapper, WMCasinoAgentMapper>()
                .AddTransient<IWMCasinoUtility, WMCasinoUtility>()
                .AddTransient<IWMCasinoGamePlayer, WMCasinoGamePlayer>();

            services
                .AddTransient<IAsiaGamingService, AsiaGamingService>()
                .AddTransient<IAsiaGamingGameSession, AsiaGamingGameSession>()
                .AddTransient<IAsiaGamingGamePlayer, AsiaGamingGamePlayer>();

            services
                .AddTransient<IPragmaticPlayService, PragmaticPlayService>();

            services
                .AddTransient<IIMESportsMerchantMapper, IMESportsMerchantMapper>()
                .AddTransient<IIMESportsGamePlayer, IMESportsGamePlayer>()
                .AddTransient<IIMESportsUtility, IMESportsUtility>()
                .AddTransient<IIMESportsService, IMESportsService>();

            services
                .AddTransient<IIMESportsDataStore>(sp => ActivatorUtilities.CreateInstance<IMESportsRedisDataStore>(sp, redisDatabaseResolver(sp)));

            services
                .AddTransient<ISabaService, SabaService>()
                .AddTransient<ISabaGamePlayer, SabaGamePlayer>();

            return services;
        }
    }
}