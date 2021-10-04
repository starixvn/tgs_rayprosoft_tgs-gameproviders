using TGS.GameProviders.ApiClients.BigGaming;
using TGS.GameProviders.ApiClients.Evolution;
using TGS.GameProviders.ApiClients.KY;
using TGS.GameProviders.ApiClients.SexyGaming;
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
                .AddScoped<IBigGamingUtility, BigGamingUtility>();

            return services.AddTransient<IKYApiClient, KYApiClient>()
                .AddTransient<IVRApiClient, VRApiClient>()
                .AddTransient<IWGHorseApiClient, WGHorseApiClient>()
                .AddTransient<IWGSportsApiClient, WGSportsApiClient>()
                .AddTransient<IBigGamingApiClient, BigGamingApiClient>()
                .AddTransient<IEvolutionApiClient, EvolutionApiClient>()
                .AddTransient<ISexyGamingApiClient, SexyGamingApiClient>();
        }
    }
}
