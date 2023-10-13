using Tgs.GameCommand.Client;
using Tgs.GameInterface.AVIAGaming.Client;
using Tgs.GameInterface.AwcService.Client;
using Tgs.GameInterface.BTiService.Client;
using Tgs.GameInterface.Cards365.Client;
using Tgs.GameInterface.DreamGamingService.Client;
using Tgs.GameInterface.EndorphinaService.Client;
using Tgs.GameInterface.EvolutionService.Client;
using Tgs.GameInterface.FaChai.Client;
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
using Tgs.GameInterface.V8Poker.Client;
using Tgs.GameInterface.WGFmGamingService.Client;
using Tgs.GameInterface.WGViaGaming.Client;
using Tgs.GameInterface.WicketCricket.Client;
using Tgs.GameInterface.WMCasinoService.Client;
using Tgs.GameInterface.YeeBet.Client;
using Tgs.GameProviders;
using Tgs.GameRecord.Endorphina.Session.Client;
using Tgs.GameRounds.Client;
using Tgs.GameToken.Client;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGameProvidersService(
        this IServiceCollection services,
        Action<RedisOptions, IServiceProvider> configure,
        Action<GameRoundsClientOptions, IServiceProvider> gameRoundsClientOptions,
        Action<GameTokenClientOptions, IServiceProvider> gameTokenClientOptions,
        Action<GameCommandClientOptions, IServiceProvider> gameCommandClientOptions,
        Action<GameInterfaceAwcServiceClientOptions, IServiceProvider> gameInterfaceAwcServiceClientOptions,
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
        Action<GameInterfaceAVIAGamingClientOptions, IServiceProvider> gameInterfaceAVIAGamingClientOptions,
        Action<GameInterfaceKaiyuanClientOptions, IServiceProvider> gameInterfaceKaiyuanClientOptions,
        Action<GameInterfaceWicketCricketClientOptions, IServiceProvider> gameInterfaceWicketCricketClientOptions,
        Action<GameInterfaceGKXGamingClientOptions, IServiceProvider> gameInterfaceGKXGamingClientOptions,
        Action<GameInterfaceV8PokerClientOptions, IServiceProvider> gameInterfaceV8PokerClientOptions)
        => services
        .AddOptions<RedisOptions>()
        .Configure(configure)
        .Services
        .AddSingleton<RedisConnectionFactory>()
        .AddGameProvidersRepositories()
        .AddGameProvidersServices((sp) => sp.GetRequiredService<RedisConnectionFactory>().Database)
        .AddGameProvidersCore(
            gameRoundsClientOptions: gameRoundsClientOptions,
            gameTokenClientOptions: gameTokenClientOptions,
            gameCommandClientOptions: gameCommandClientOptions,
            gameInterfaceAwcServiceClientOptions: gameInterfaceAwcServiceClientOptions,
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
            gameInterfaceAVIAGamingClientOptions: gameInterfaceAVIAGamingClientOptions,
            gameInterfaceKaiyuanClientOptions: gameInterfaceKaiyuanClientOptions,
            gameInterfaceWicketCricketClientOptions: gameInterfaceWicketCricketClientOptions,
            gameInterfaceGKXGamingClientOptions: gameInterfaceGKXGamingClientOptions,
            gameInterfaceV8PokerClientOptions: gameInterfaceV8PokerClientOptions);
}
