// ReSharper disable CheckNamespace

using TGS.GameProviders.ApiClients;
using TGS.GameProviders.ApiClients.BigGaming;
using TGS.GameProviders.ApiClients.Evolution;
using TGS.GameProviders.ApiClients.KY;
using TGS.GameProviders.ApiClients.SBO;
using TGS.GameProviders.ApiClients.SexyGaming;
using TGS.GameProviders.ApiClients.Skywind;
using TGS.GameProviders.ApiClients.TCG;
using TGS.GameProviders.ApiClients.VR;
using TGS.GameProviders.ApiClients.WGHorse;
using TGS.GameProviders.ApiClients.WGSports;

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
                .AddScoped<ITCGUtility, TCGUtility>();

            services.AddHttpClient<IKYApiClient, KYApiClient>();
            services.AddHttpClient<IVRApiClient, VRApiClient>();
            services.AddHttpClient<IWGHorseApiClient, WGHorseApiClient>();
            services.AddHttpClient<IWGSportsApiClient, WGSportsApiClient>();
            services.AddHttpClient<IBigGamingApiClient, BigGamingApiClient>();
            services.AddHttpClient<IEvolutionApiClient, EvolutionApiClient>();
            services.AddHttpClient<ISexyGamingApiClient, SexyGamingApiClient>();
            services.AddHttpClient<ISkywindApiClient, SkywindApiClient>();
            services.AddHttpClient<ITCGApiClient, TCGApiClient>();
            services.AddHttpClient<ISBOApiClient, SBOApiClient>();

            return services;
        }
    }
}