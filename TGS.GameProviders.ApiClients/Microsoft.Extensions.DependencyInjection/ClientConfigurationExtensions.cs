using TGS.GameProviders.ApiClients.BigGaming;
using TGS.GameProviders.ApiClients.Evolution;
using TGS.GameProviders.ApiClients.KY;
using TGS.GameProviders.ApiClients.LY;
using TGS.GameProviders.ApiClients.SBO;
using TGS.GameProviders.ApiClients.SexyGaming;
using TGS.GameProviders.ApiClients.Skywind;
using TGS.GameProviders.ApiClients.TCG;
using TGS.GameProviders.ApiClients.VR;
using TGS.GameProviders.ApiClients.WGHorse;
using TGS.GameProviders.ApiClients.WGSports;
using TGS.GameProviders.ApiClients.WMCasino;
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
    public static class ClientConfigurationExtensions
    {
        public static IServiceCollection AddApiClients(this IServiceCollection services)
        {
            services.AddScoped<IKYUtility, KYUtility>()
                .AddScoped<IVRUtility, VRUtility>()
                .AddScoped<IWGHorseUtility, WGHorseUtility>()
                .AddScoped<IWGSportsUtility, WGSportsUtility>()
                .AddScoped<IBigGamingUtility, BigGamingUtility>()
                .AddScoped<ITCGUtility, TCGUtility>()
                .AddScoped<ILYUtility, LYUtility>();

            services.AddHttpClient<IKYApiClient, KYApiClient>();
            services.AddHttpClient<IVRApiClient, VRApiClient>();
            services.AddHttpClient<IWGHorseApiClient, WGHorseApiClient>();
            services.AddHttpClient<IWGSportsApiClient, WGSportsApiClient>();
            services.AddHttpClient<IBigGamingApiClient, BigGamingApiClient>();
            services.AddHttpClient<IEvolutionApiClient, EvolutionApiClient>();
            services.AddHttpClient<ISexyGamingApiClient, SexyGamingApiClient>();
            services.AddHttpClient<ISkywindApiClient, SkywindApiClient>();
            services.AddHttpClient<ITCGApiClient, TCGApiClient>();
            services.AddHttpClient<ILYApiClient, LYApiClient>();
            services.AddHttpClient<ISBOApiClient, SBOApiClient>();
            services.AddHttpClient<IWMCasinoApiClient, WMCasinoApiClient>();

            return services;
        }
    }
}
