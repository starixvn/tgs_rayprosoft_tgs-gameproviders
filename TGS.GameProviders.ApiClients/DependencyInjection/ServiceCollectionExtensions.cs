using TGS.GameProviders.ApiClients.AllBet;
using TGS.GameProviders.ApiClients.AsiaGaming;
using TGS.GameProviders.ApiClients.Avia;
using TGS.GameProviders.ApiClients.BigGaming;
using TGS.GameProviders.ApiClients.Card365;
using TGS.GameProviders.ApiClients.DreamGame;
using TGS.GameProviders.ApiClients.Evolution;
using TGS.GameProviders.ApiClients.Habanero;
using TGS.GameProviders.ApiClients.IMESports;
using TGS.GameProviders.ApiClients.JDB;
using TGS.GameProviders.ApiClients.Jili;
using TGS.GameProviders.ApiClients.KY;
using TGS.GameProviders.ApiClients.LY;
using TGS.GameProviders.ApiClients.PragmaticPlay;
using TGS.GameProviders.ApiClients.Saba;
using TGS.GameProviders.ApiClients.SBO;
using TGS.GameProviders.ApiClients.SexyGaming;
using TGS.GameProviders.ApiClients.Skywind;
using TGS.GameProviders.ApiClients.TCG;
using TGS.GameProviders.ApiClients.TF;
using TGS.GameProviders.ApiClients.VR;
using TGS.GameProviders.ApiClients.WGHorse;
using TGS.GameProviders.ApiClients.WGSports;
using TGS.GameProviders.ApiClients.WMCasino;
using TGS.GameProviders.Core.AllBet;
using TGS.GameProviders.Core.AsiaGaming;
using TGS.GameProviders.Core.Avia;
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

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiClients(this IServiceCollection services)
    {
        _ = services
            .AddScoped<IAsiaGamingUtility, AsiaGamingUtility>()
            .AddScoped<IPragmaticPlayUtility, PragmaticPlayUtility>();

        _ = services
            .AddScoped<IBigGamingRoundDetailAnalyzeService, BigGamingRoundDetailAnalyzeService>();

        _ = services
            .AddScoped<IEvolutionRoundDetailAnalyzeService, EvolutionRoundDetailAnalyzeService>();

        _ = services
            .AddScoped<ISexyGamingRoundDetailAnalyzeService, SexyGamingRoundDetailAnalyzeService>();

        _ = services.AddHttpClient<IKYApiClient, KYApiClient>();
        _ = services.AddHttpClient<IVRApiClient, VRApiClient>();
        _ = services.AddHttpClient<IWGHorseApiClient, WGHorseApiClient>();
        _ = services.AddHttpClient<IWGSportsApiClient, WGSportsApiClient>();
        _ = services.AddHttpClient<IBigGamingApiClient, BigGamingApiClient>();
        _ = services.AddHttpClient<IEvolutionApiClient, EvolutionApiClient>();
        _ = services.AddHttpClient<ISexyGamingApiClient, SexyGamingApiClient>();
        _ = services.AddHttpClient<ISkywindApiClient, SkywindApiClient>();
        _ = services.AddHttpClient<ITCGApiClient, TCGApiClient>();
        _ = services.AddHttpClient<ILYApiClient, LYApiClient>();
        _ = services.AddHttpClient<ISBOApiClient, SBOApiClient>();
        _ = services.AddHttpClient<IWMCasinoApiClient, WMCasinoApiClient>();
        _ = services.AddHttpClient<IAsiaGamingApiClient, AsiaGamingApiClient>();
        _ = services.AddHttpClient<IPragmaticPlayApiClient, PragmaticPlayApiClient>();
        _ = services.AddHttpClient<IIMESportsApiClient, IMESportsApiClient>();
        _ = services.AddHttpClient<ISabaApiClient, SabaApiClient>();
        _ = services.AddHttpClient<IAllBetApiClient, AllBetApiClient>();
        _ = services.AddHttpClient<IAviaApiClient, AviaApiClient>();
        _ = services.AddHttpClient<IJiliApiClient, JiliApiClient>();
        _ = services.AddHttpClient<ITFApiClient, TFApiClient>();
        _ = services.AddHttpClient<IHabaneroApiClient, HabaneroApiClient>();
        _ = services.AddHttpClient<ICard365ApiClient, Card365ApiClient>();
        _ = services.AddHttpClient<IJDBApiClient, JDBApiClient>();
        _ = services.AddHttpClient<IDreamGameApiClient, DreamGameApiClient>();

        return services;
    }
}
