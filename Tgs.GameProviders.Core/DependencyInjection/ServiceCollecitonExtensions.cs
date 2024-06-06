using System;
using System.Reflection;
using Tgs.GameCommand.Client;
using Tgs.GameInterface.Awc.Client;
using Tgs.GameInterface.BTiService.Client;
using Tgs.GameInterface.Cards365.Client;
using Tgs.GameInterface.CQ9Gaming.Client;
using Tgs.GameInterface.CrPati.Client;
using Tgs.GameInterface.DreamGamingService.Client;
using Tgs.GameInterface.EndorphinaService.Client;
using Tgs.GameInterface.EvolutionService.Client;
using Tgs.GameInterface.FaChai.Client;
using Tgs.GameInterface.Funky.Client;
using Tgs.GameInterface.GKXGaming.Client;
using Tgs.GameInterface.HabaneroService.Client;
using Tgs.GameInterface.InplayMatrix.Client;
using Tgs.GameInterface.JDBService.Client;
using Tgs.GameInterface.Jili.Client;
using Tgs.GameInterface.Joker.Client;
using Tgs.GameInterface.Kaiyuan.Client;
using Tgs.GameInterface.LEGaming.Client;
using Tgs.GameInterface.OBet.Client;
using Tgs.GameInterface.Pinnacle.Client;
using Tgs.GameInterface.PragmaticPlay.Client;
using Tgs.GameInterface.SabaService.Client;
using Tgs.GameInterface.SBOService.Client;
using Tgs.GameInterface.SkywindService.Client;
using Tgs.GameInterface.SM01Service.Client;
using Tgs.GameInterface.TFGamingService.Client;
using Tgs.GameInterface.TPhere.Client;
using Tgs.GameInterface.V8Poker.Client;
using Tgs.GameInterface.VRGaming.Client;
using Tgs.GameInterface.WGFmGamingService.Client;
using Tgs.GameInterface.WGViaGaming.Client;
using Tgs.GameInterface.WicketCricket.Client;
using Tgs.GameInterface.WMCasinoService.Client;
using Tgs.GameInterface.YeeBet.Client;
using Tgs.GameProviders.Core;
using Tgs.GameRecord.Endorphina.Session.Client;
using Tgs.GameToken.Client;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollecitonExtensions
{
	public static IServiceCollection AddGameProvidersCore(
		this IServiceCollection services,
		Action<IPAddressOptions, IServiceProvider> ipAddressOptions,
		Action<GameTokenClientOptions, IServiceProvider> gameTokenClientOptions,
		Action<GameCommandClientOptions, IServiceProvider> gameCommandClientOptions,
		Action<GameInterfaceAwcClientOptions, IServiceProvider> gameInterfaceAwcClientOptions,
		Action<GameInterfaceJiliClientOptions, IServiceProvider> gameInterfaceJiliClientOptions,
		Action<GameInterfaceWMCasinoServiceClientOptions, IServiceProvider> gameInterfaceWMCasinoClientOptions,
		Action<GameInterfaceEvolutionClientOptions, IServiceProvider> gameInterfaceEvolutionServiceClientOptions,
		Action<GameInterfaceSkywindClientOptions, IServiceProvider> gameInterfaceSkywindClientOptions,
		Action<GameInterfaceWGFmGamingClientOptions, IServiceProvider> gameInterfaceWGFmGamingClientOptions,
		Action<GameInterfaceJDBClientOptions, IServiceProvider> gameInterfaceJDBClientOptions,
		Action<GameInterfaceSBOClientOptions, IServiceProvider> gameInterfaceSBOClientOptions,
		Action<GameInterfaceBTiClientOptions, IServiceProvider> gameInterfaceBTiClientOptions,
		Action<GameInterfaceHabaneroClientOptions, IServiceProvider> gameInterfaceHabaneroClientOptions,
		Action<GameInterfaceSabaClientOptions, IServiceProvider> gameInterfaceSabaClientOptions,
		Action<GameInterfaceSM01ClientOptions, IServiceProvider> gameInterfaceSM01ClientOptions,
		Action<GameInterfaceTFGamingClientOptions, IServiceProvider> gameInterfaceTFGamingClientOptions,
		Action<GameInterfaceDreamGamingClientOptions, IServiceProvider> gameInterfaceDreamGamingClientOptions,
		Action<GameInterfaceEndorphinaClientOptions, IServiceProvider> gameInterfaceEndorphinaClientOptions,
		Action<GameRecordEndorphinaSessionClientOptions, IServiceProvider> gameRecordEndorphinaClientOptions,
		Action<GameInterfaceFaChaiClientOptions, IServiceProvider> gameInterfaceFaChaiClientOptions,
		Action<GameInterfaceOBetClientOptions, IServiceProvider> gameInterfaceOBetClientOptions,
		Action<GameInterfaceYeeBetClientOptions, IServiceProvider> gameInterfaceYeeBetClientOptions,
		Action<GameInterfacePragmaticPlayClientOptions, IServiceProvider> gameInterfacePragmaticPlayClientOptions,
		Action<GameInterfaceInplayMatrixClientOptions, IServiceProvider> gameInterfaceInplayMatrixClientOptions,
		Action<GameInterfaceCards365ClientOptions, IServiceProvider> gameInterfaceCards365ClientOptions,
		Action<GameInterfacePinnacleClientOptions, IServiceProvider> gameInterfacePinnacleClientOptions,
		Action<GameInterfaceJokerClientOptions, IServiceProvider> gameInterfaceJokerClientOptions,
		Action<GameInterfaceWGViaGamingClientOptions, IServiceProvider> gameInterfaceWGViaGamingClientOptions,
		Action<GameInterfaceLEGamingClientOptions, IServiceProvider> gameInterfaceLEGamingClientOptions,
		Action<GameInterfaceKaiyuanClientOptions, IServiceProvider> gameInterfaceKaiyuanClientOptions,
		Action<GameInterfaceWicketCricketClientOptions, IServiceProvider> gameInterfaceWicketCricketClientOptions,
		Action<GameInterfaceGKXGamingClientOptions, IServiceProvider> gameInterfaceGKXGamingClientOptions,
		Action<GameInterfaceV8PokerClientOptions, IServiceProvider> gameInterfaceV8PokerClientOptions,
		Action<GameInterfaceCrPatiClientOptions, IServiceProvider> gameInterfaceCrPatiClientOptions,
		Action<GameInterfaceVRGamingClientOptions, IServiceProvider> gameInterfaceVRGamingClientOptions,
		Action<GameInterfaceFunkyClientOptions, IServiceProvider> gameInterfaceFunkyClientOptions,
		Action<GameInterfaceTPhereClientOptions, IServiceProvider> gameInterfaceTPhereClientOptions,
		Action<GameInterfaceCQ9GamingClientOptions, IServiceProvider> gameInterfaceCQ9GamingClientOptions)
		=> services
		.AddOptions<IPAddressOptions>()
		.Configure(ipAddressOptions)
		.Services
		.RegisterAllServiceUtilityTypes(
			Assembly.GetExecutingAssembly(),
			ServiceLifetime.Transient)
		.AddGameCommandClient(gameCommandClientOptions)
		.AddGameInterfaceAwcClient(gameInterfaceAwcClientOptions)
		.AddGameInterfaceJiliClient(gameInterfaceJiliClientOptions)
		.AddGameInterfaceWMCasinoServiceClient(gameInterfaceWMCasinoClientOptions)
		.AddGameInterfaceEvoClient(gameInterfaceEvolutionServiceClientOptions)
		.AddGameInterfaceSkywindServiceClient(gameInterfaceSkywindClientOptions)
		.AddGameInterfaceWGFmGamingServiceClient(gameInterfaceWGFmGamingClientOptions)
		.AddGameInterfaceWGViaGamingClient(gameInterfaceWGViaGamingClientOptions)
		.AddGameInterfaceJDBServiceClient(gameInterfaceJDBClientOptions)
		.AddGameInterfaceSBOServiceClient(gameInterfaceSBOClientOptions)
		.AddGameInterfaceBTiServiceClient(gameInterfaceBTiClientOptions)
		.AddGameInterfaceHabaneroServiceClient(gameInterfaceHabaneroClientOptions)
		.AddGameInterfaceSabaServiceClient(gameInterfaceSabaClientOptions)
		.AddGameInterfaceSM01ServiceClient(gameInterfaceSM01ClientOptions)
		.AddGameInterfaceTFGamingServiceClient(gameInterfaceTFGamingClientOptions)
		.AddGameInterfaceDreamGamingServiceClient(gameInterfaceDreamGamingClientOptions)
		.AddGameInterfaceEndorphinaServiceClient(gameInterfaceEndorphinaClientOptions)
		.AddGameInterfaceFaChaiClient(gameInterfaceFaChaiClientOptions)
		.AddGameInterfaceOBetClient(gameInterfaceOBetClientOptions)
		.AddGameInterfaceYeeBetClient(gameInterfaceYeeBetClientOptions)
		.AddGameRecordEndorphinaSessionClient(gameRecordEndorphinaClientOptions)
		.AddGameInterfacePragmaticPlayClient(gameInterfacePragmaticPlayClientOptions)
		.AddGameInterfaceInplayMatrixClient(gameInterfaceInplayMatrixClientOptions)
		.AddGameInterfaceCards365Client(gameInterfaceCards365ClientOptions)
		.AddGameInterfacePinnacleClient(gameInterfacePinnacleClientOptions)
		.AddGameInterfaceJokerClient(gameInterfaceJokerClientOptions)
		.AddGameInterfaceLEGamingClient(gameInterfaceLEGamingClientOptions)
		.AddGameInterfaceKaiyuanClient(gameInterfaceKaiyuanClientOptions)
		.AddGameInterfaceWicketCricketClient(gameInterfaceWicketCricketClientOptions)
		.AddGameInterfaceGKXGamingClient(gameInterfaceGKXGamingClientOptions)
		.AddGameInterfaceV8PokerClient(gameInterfaceV8PokerClientOptions)
		.AddGameInterfaceCrPatiClient(gameInterfaceCrPatiClientOptions)
		.AddGameInterfaceVRGamingClient(gameInterfaceVRGamingClientOptions)
		.AddGameInterfaceFunkyClient(gameInterfaceFunkyClientOptions)
		.AddGameInterfaceTPhereClient(gameInterfaceTPhereClientOptions)
		.AddGameInterfaceCQ9GamingClient(gameInterfaceCQ9GamingClientOptions)
		.AddGameTokenClient(gameTokenClientOptions)
		//Tgs.Core.Infrastructures.Security
		.AddSecurity()
		.AddMemoryCache();
}
