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
using Tgs.GameInterface.Ga28.Client;
using Tgs.GameInterface.GKXGaming.Client;
using Tgs.GameInterface.GlobalTechnology.Client;
using Tgs.GameInterface.HabaneroService.Client;
using Tgs.GameInterface.InplayMatrix.Client;
using Tgs.GameInterface.JDBService.Client;
using Tgs.GameInterface.Jili.Client;
using Tgs.GameInterface.Joker.Client;
using Tgs.GameInterface.Kaiyuan.Client;
using Tgs.GameInterface.LEGaming.Client;
using Tgs.GameInterface.MicroGaming.Client;
using Tgs.GameInterface.OBet.Client;
using Tgs.GameInterface.Pinnacle.Client;
using Tgs.GameInterface.PragmaticPlay.Client;
using Tgs.GameInterface.SabaService.Client;
using Tgs.GameInterface.SBOService.Client;
using Tgs.GameInterface.SharpSports.Client;
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
using Tgs.GameProviders;
using Tgs.GameProviders.Core;
using Tgs.GameRecord.Endorphina.Session.Client;
using Tgs.GameToken.Client;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddGameProviders(
		this IServiceCollection services,
		Action<RedisOptions, IServiceProvider> configure,
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
		Action<GameRecordEndorphinaSessionClientOptions, IServiceProvider> gameRecordEndorphinaSessionClientOptions,
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
		Action<GameInterfaceCQ9GamingClientOptions, IServiceProvider> gameInterfaceCQ9GamingClientOptions,
		Action<GameInterfaceSharpSportsClientOptions, IServiceProvider> gameInterfaceSharpSportsClientOptions,
		Action<GameInterfaceGa28ClientOptions, IServiceProvider> gameInterfaceGa28ClientOptions,
		Action<GameInterfaceGlobalTechnologyClientOptions, IServiceProvider> gameInterfaceGlobalTechnologyClientOptions,
		Action<GameInterfaceMicroGamingClientOptions, IServiceProvider> gameInterfaceMicroGamingClientOptions)
		=> services
		.AddOptions<RedisOptions>()
		.Configure(configure)
		.Services
		.AddSingleton<RedisConnectionFactory>()
		.AddGameProvidersRepositories()
		.AddGameProvidersCore(
			ipAddressOptions: ipAddressOptions,
			gameTokenClientOptions: gameTokenClientOptions,
			gameCommandClientOptions: gameCommandClientOptions,
			gameInterfaceAwcClientOptions: gameInterfaceAwcClientOptions,
			gameInterfaceJiliClientOptions: gameInterfaceJiliClientOptions,
			gameInterfaceWMCasinoClientOptions: gameInterfaceWMCasinoClientOptions,
			gameInterfaceEvolutionServiceClientOptions: gameInterfaceEvolutionServiceClientOptions,
			gameInterfaceSkywindClientOptions: gameInterfaceSkywindClientOptions,
			gameInterfaceWGFmGamingClientOptions: gameInterfaceWGFmGamingClientOptions,
			gameInterfaceJDBClientOptions: gameInterfaceJDBClientOptions,
			gameInterfaceSBOClientOptions: gameInterfaceSBOClientOptions,
			gameInterfaceBTiClientOptions: gameInterfaceBTiClientOptions,
			gameInterfaceHabaneroClientOptions: gameInterfaceHabaneroClientOptions,
			gameInterfaceSabaClientOptions: gameInterfaceSabaClientOptions,
			gameInterfaceSM01ClientOptions: gameInterfaceSM01ClientOptions,
			gameInterfaceTFGamingClientOptions: gameInterfaceTFGamingClientOptions,
			gameInterfaceDreamGamingClientOptions: gameInterfaceDreamGamingClientOptions,
			gameInterfaceEndorphinaClientOptions: gameInterfaceEndorphinaClientOptions,
			gameRecordEndorphinaClientOptions: gameRecordEndorphinaSessionClientOptions,
			gameInterfaceFaChaiClientOptions: gameInterfaceFaChaiClientOptions,
			gameInterfaceOBetClientOptions: gameInterfaceOBetClientOptions,
			gameInterfaceYeeBetClientOptions: gameInterfaceYeeBetClientOptions,
			gameInterfacePragmaticPlayClientOptions: gameInterfacePragmaticPlayClientOptions,
			gameInterfaceInplayMatrixClientOptions: gameInterfaceInplayMatrixClientOptions,
			gameInterfaceCards365ClientOptions: gameInterfaceCards365ClientOptions,
			gameInterfacePinnacleClientOptions: gameInterfacePinnacleClientOptions,
			gameInterfaceJokerClientOptions: gameInterfaceJokerClientOptions,
			gameInterfaceWGViaGamingClientOptions: gameInterfaceWGViaGamingClientOptions,
			gameInterfaceLEGamingClientOptions: gameInterfaceLEGamingClientOptions,
			gameInterfaceKaiyuanClientOptions: gameInterfaceKaiyuanClientOptions,
			gameInterfaceWicketCricketClientOptions: gameInterfaceWicketCricketClientOptions,
			gameInterfaceGKXGamingClientOptions: gameInterfaceGKXGamingClientOptions,
			gameInterfaceV8PokerClientOptions: gameInterfaceV8PokerClientOptions,
			gameInterfaceCrPatiClientOptions: gameInterfaceCrPatiClientOptions,
			gameInterfaceVRGamingClientOptions: gameInterfaceVRGamingClientOptions,
			gameInterfaceFunkyClientOptions: gameInterfaceFunkyClientOptions,
			gameInterfaceTPhereClientOptions: gameInterfaceTPhereClientOptions,
			gameInterfaceCQ9GamingClientOptions: gameInterfaceCQ9GamingClientOptions,
			gameInterfaceSharpSportsClientOptions: gameInterfaceSharpSportsClientOptions,
			gameInterfaceGa28ClientOptions: gameInterfaceGa28ClientOptions,
			gameInterfaceGlobalTechnologyClientOptions: gameInterfaceGlobalTechnologyClientOptions,
			gameInterfaceMicroGamingClientOptions: gameInterfaceMicroGamingClientOptions);
}
