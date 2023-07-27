using Tgs.GameProviders.ApiClients.AllBet;
using Tgs.GameProviders.ApiClients.AsiaGaming;
using Tgs.GameProviders.ApiClients.BigGaming;
using Tgs.GameProviders.ApiClients.Card365;
using Tgs.GameProviders.ApiClients.DreamGame;
using Tgs.GameProviders.ApiClients.Evolution;
using Tgs.GameProviders.ApiClients.Habanero;
using Tgs.GameProviders.ApiClients.IMESports;
using Tgs.GameProviders.ApiClients.JDB;
using Tgs.GameProviders.ApiClients.Jili;
using Tgs.GameProviders.ApiClients.PragmaticPlay;
using Tgs.GameProviders.ApiClients.Saba;
using Tgs.GameProviders.ApiClients.SBO;
using Tgs.GameProviders.ApiClients.SexyGaming;
using Tgs.GameProviders.ApiClients.Skywind;
using Tgs.GameProviders.ApiClients.TCG;
using Tgs.GameProviders.ApiClients.TF;
using Tgs.GameProviders.ApiClients.VR;
using Tgs.GameProviders.ApiClients.WMCasino;
using Tgs.GameProviders.Core.AllBet;
using Tgs.GameProviders.Core.AsiaGaming;
using Tgs.GameProviders.Core.BigGaming;
using Tgs.GameProviders.Core.Card365;
using Tgs.GameProviders.Core.DreamGame;
using Tgs.GameProviders.Core.Evolution;
using Tgs.GameProviders.Core.Habanero;
using Tgs.GameProviders.Core.IMESports;
using Tgs.GameProviders.Core.JDB;
using Tgs.GameProviders.Core.Jili;
using Tgs.GameProviders.Core.PragmaticPlay;
using Tgs.GameProviders.Core.Saba;
using Tgs.GameProviders.Core.SBO;
using Tgs.GameProviders.Core.SexyGaming;
using Tgs.GameProviders.Core.Skywind;
using Tgs.GameProviders.Core.TCG;
using Tgs.GameProviders.Core.TFGaming;
using Tgs.GameProviders.Core.VR;
using Tgs.GameProviders.Core.WMCasino;

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

        _ = services.AddHttpClient<IVRApiClient, VRApiClient>();
        _ = services.AddHttpClient<IBigGamingApiClient, BigGamingApiClient>();
        _ = services.AddHttpClient<IEvolutionApiClient, EvolutionApiClient>();
        _ = services.AddHttpClient<ISexyGamingApiClient, SexyGamingApiClient>();
        _ = services.AddHttpClient<ISkywindApiClient, SkywindApiClient>();
        _ = services.AddHttpClient<ITCGApiClient, TCGApiClient>();
        _ = services.AddHttpClient<ISBOApiClient, SBOApiClient>();
        _ = services.AddHttpClient<IWMCasinoApiClient, WMCasinoApiClient>();
        _ = services.AddHttpClient<IAsiaGamingApiClient, AsiaGamingApiClient>();
        _ = services.AddHttpClient<IPragmaticPlayApiClient, PragmaticPlayApiClient>();
        _ = services.AddHttpClient<IIMESportsApiClient, IMESportsApiClient>();
        _ = services.AddHttpClient<ISabaApiClient, SabaApiClient>();
        _ = services.AddHttpClient<IAllBetApiClient, AllBetApiClient>();
        _ = services.AddHttpClient<IJiliApiClient, JiliApiClient>();
        _ = services.AddHttpClient<ITFApiClient, TFApiClient>();
        _ = services.AddHttpClient<IHabaneroApiClient, HabaneroApiClient>();
        _ = services.AddHttpClient<ICard365ApiClient, Card365ApiClient>();
        _ = services.AddHttpClient<IJDBApiClient, JDBApiClient>();
        _ = services.AddHttpClient<IDreamGameApiClient, DreamGameApiClient>();

        return services;
    }
}
