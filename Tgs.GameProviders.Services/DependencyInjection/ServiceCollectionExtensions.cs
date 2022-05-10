using System;
using StackExchange.Redis;
using Tgs.GameProviders.Core.AllBet;
using Tgs.GameProviders.Core.AsiaGaming;
using Tgs.GameProviders.Core.Avia;
using Tgs.GameProviders.Core.BetConstruct;
using Tgs.GameProviders.Core.BigGaming;
using Tgs.GameProviders.Core.Card365;
using Tgs.GameProviders.Core.DreamGame;
using Tgs.GameProviders.Core.Evolution;
using Tgs.GameProviders.Core.Habanero;
using Tgs.GameProviders.Core.IMESports;
using Tgs.GameProviders.Core.JDB;
using Tgs.GameProviders.Core.Jili;
using Tgs.GameProviders.Core.Joker;
using Tgs.GameProviders.Core.KY;
using Tgs.GameProviders.Core.LY;
using Tgs.GameProviders.Core.PragmaticPlay;
using Tgs.GameProviders.Core.Saba;
using Tgs.GameProviders.Core.SBO;
using Tgs.GameProviders.Core.SexyGaming;
using Tgs.GameProviders.Core.Skywind;
using Tgs.GameProviders.Core.TCG;
using Tgs.GameProviders.Core.TFGaming;
using Tgs.GameProviders.Core.VR;
using Tgs.GameProviders.Core.WGHorse;
using Tgs.GameProviders.Core.WGSports;
using Tgs.GameProviders.Core.WMCasino;
using Tgs.GameProviders.Services.AllBet;
using Tgs.GameProviders.Services.AsiaGaming;
using Tgs.GameProviders.Services.Avia;
using Tgs.GameProviders.Services.BetConstruct;
using Tgs.GameProviders.Services.BigGaming;
using Tgs.GameProviders.Services.Card365;
using Tgs.GameProviders.Services.DreamGame;
using Tgs.GameProviders.Services.Evolution;
using Tgs.GameProviders.Services.Habanero;
using Tgs.GameProviders.Services.IMESports;
using Tgs.GameProviders.Services.JDB;
using Tgs.GameProviders.Services.Jili;
using Tgs.GameProviders.Services.Joker;
using Tgs.GameProviders.Services.KY;
using Tgs.GameProviders.Services.LY;
using Tgs.GameProviders.Services.PragmaticPlay;
using Tgs.GameProviders.Services.Saba;
using Tgs.GameProviders.Services.SBO;
using Tgs.GameProviders.Services.SexyGaming;
using Tgs.GameProviders.Services.Skywind;
using Tgs.GameProviders.Services.TCG;
using Tgs.GameProviders.Services.TFGaming;
using Tgs.GameProviders.Services.VR;
using Tgs.GameProviders.Services.WGHorse;
using Tgs.GameProviders.Services.WGSports;
using Tgs.GameProviders.Services.WMCasino;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
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
                .AddTransient<IWGHorseGamePlayer, WGHorseGamePlayer>();
            _ = services
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

            _ = services
                .AddTransient<IJokerAgentMapper, JokerAgentMapper>()
                .AddTransient<IJokerService, JokerService>()
                .AddTransient<IJokerGamePlayer, JokerGamePlayer>();

            return services;
        }
    }
}