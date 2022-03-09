using System;
using StackExchange.Redis;
using TGS.GameProviders.Core.AllBet;
using TGS.GameProviders.Core.AsiaGaming;
using TGS.GameProviders.Core.Avia;
using TGS.GameProviders.Core.BetConstruct;
using TGS.GameProviders.Core.BigGaming;
using TGS.GameProviders.Core.Card365;
using TGS.GameProviders.Core.DreamGame;
using TGS.GameProviders.Core.Evolution;
using TGS.GameProviders.Core.Habanero;
using TGS.GameProviders.Core.IMESports;
using TGS.GameProviders.Core.JDB;
using TGS.GameProviders.Core.Jili;
using TGS.GameProviders.Core.KY;
using TGS.GameProviders.Core.LY;
using TGS.GameProviders.Core.PragmaticPlay;
using TGS.GameProviders.Core.Saba;
using TGS.GameProviders.Core.SBO;
using TGS.GameProviders.Core.SexyGaming;
using TGS.GameProviders.Core.Skywind;
using TGS.GameProviders.Core.TCG;
using TGS.GameProviders.Core.TFGaming;
using TGS.GameProviders.Core.VR;
using TGS.GameProviders.Core.WGHorse;
using TGS.GameProviders.Core.WGSports;
using TGS.GameProviders.Core.WMCasino;
using TGS.GameProviders.Services.AllBet;
using TGS.GameProviders.Services.AsiaGaming;
using TGS.GameProviders.Services.Avia;
using TGS.GameProviders.Services.BetConstruct;
using TGS.GameProviders.Services.BigGaming;
using TGS.GameProviders.Services.Card365;
using TGS.GameProviders.Services.DreamGame;
using TGS.GameProviders.Services.Evolution;
using TGS.GameProviders.Services.Habanero;
using TGS.GameProviders.Services.IMESports;
using TGS.GameProviders.Services.JDB;
using TGS.GameProviders.Services.Jili;
using TGS.GameProviders.Services.KY;
using TGS.GameProviders.Services.LY;
using TGS.GameProviders.Services.PragmaticPlay;
using TGS.GameProviders.Services.Saba;
using TGS.GameProviders.Services.SBO;
using TGS.GameProviders.Services.SexyGaming;
using TGS.GameProviders.Services.Skywind;
using TGS.GameProviders.Services.TCG;
using TGS.GameProviders.Services.TFGaming;
using TGS.GameProviders.Services.VR;
using TGS.GameProviders.Services.WGHorse;
using TGS.GameProviders.Services.WGSports;
using TGS.GameProviders.Services.WMCasino;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollecitonExtensions
    {
        public static IServiceCollection AddGameProvidersServices(
            this IServiceCollection services,
             Func<IServiceProvider, IDatabase> redisDatabaseResolver)
        {
            _ = services
                .AddTransient<IKYService, KYService>();

            _ = services
                .AddTransient<IVRService, VRService>();

            _ = services
                .AddTransient<IVRGamePlayer, VRGamePlayer>();

            _ = services
                .AddTransient<IWGHorseService, WGHorseService>()
                .AddTransient<IWGHorseGamePlayer, WGHorseGamePlayer>()
                .AddTransient<IWGSportsService, WGSportsService>()
                .AddTransient<IWGSportsGamePlayer, WGSportsGamePlayer>();

            _ = services
                .AddTransient<IBigGamingService, BigGamingService>()
                .AddTransient<IBigGamingGamePlayer, BigGamingGamePlayer>()
                .AddTransient<IBigGamingGameUrl, BigGamingGameUrl>();

            _ = services
                .AddTransient<ISexyGamingService, SexyGamingService>()
                .AddTransient<ISexyGamingGamePlayer, SexyGamingGamePlayer>();

            _ = services
                .AddTransient<ISkywindService, SkywindService>();
            _ = services
                .AddTransient<ISkywindAccessToken>(sp => ActivatorUtilities.CreateInstance<SkywindAccessToken>(sp, redisDatabaseResolver(sp)));

            _ = services
                .AddTransient<ITCGService, TCGService>()
                .AddTransient<ITCGGamePlayer, TCGGamePlayer>();

            _ = services
                .AddTransient<IEvolutionService, EvolutionService>();

            _ = services
                .AddTransient<ILYService, LYService>();

            _ = services
                .AddTransient<ISBOService, SBOService>()
                .AddTransient<ISBOGamePlayer, SBOGamePlayer>()
                .AddTransient<ISBOUtility, SBOUtility>()
                .AddTransient<ISBOGameAgent, SBOGameAgent>();

            _ = services
                .AddTransient<IWMCasinoService, WMCasinoService>()
                .AddTransient<IWMCasinoAgentMapper, WMCasinoAgentMapper>()
                .AddTransient<IWMCasinoUtility, WMCasinoUtility>()
                .AddTransient<IWMCasinoGamePlayer, WMCasinoGamePlayer>();

            _ = services
                .AddTransient<IAsiaGamingService, AsiaGamingService>()
                .AddTransient<IAsiaGamingGameSession, AsiaGamingGameSession>()
                .AddTransient<IAsiaGamingGamePlayer, AsiaGamingGamePlayer>();

            _ = services
                .AddTransient<IPragmaticPlayService, PragmaticPlayService>();

            _ = services
                .AddTransient<IIMESportsMerchantMapper, IMESportsMerchantMapper>()
                .AddTransient<IIMESportsGamePlayer, IMESportsGamePlayer>()
                .AddTransient<IIMESportsUtility, IMESportsUtility>()
                .AddTransient<IIMESportsService, IMESportsService>();

            _ = services
                .AddTransient<IIMESportsDataStore>(sp => ActivatorUtilities.CreateInstance<IMESportsRedisDataStore>(sp, redisDatabaseResolver(sp)));

            _ = services
                .AddTransient<ISabaService, SabaService>()
                .AddTransient<ISabaGamePlayer, SabaGamePlayer>()
                .AddTransient<ISabaUtility, SabaUtility>();

            _ = services
                .AddTransient<IHabaneroService, HabaneroService>();

            _ = services
                .AddTransient<IBetConstructService, BetConstructService>();

            _ = services
                .AddTransient<IAllBetService, AllBetService>()
                .AddTransient<IAllBetAgentMapper, AllBetAgentMapper>()
                .AddTransient<IAllBetGamePlayer, AllBetGamePlayer>();

            _ = services
                .AddTransient<IAviaService, AviaService>()
                .AddTransient<IAviaGamePlayerService, AviaGamePlayer>();

            _ = services
                .AddTransient<IJiliService, JiliService>()
                .AddTransient<IJiliMerchantMapper, JiliMerchantMapper>();

            _ = services
                .AddTransient<ITFGamingService, TFGamingService>();

            _ = services
                .AddTransient<ICard365Service, Card365Service>()
                .AddTransient<ICard365AgentMapper, Card365AgentMapper>();

            _ = services
                .AddTransient<IJDBService, JDBService>()
                .AddTransient<IJDBAgentMapper, JDBAgentMapper>();

            _ = services
                .AddTransient<IDreamGameService, DreamGameService>();

            return services;
        }
    }
}
