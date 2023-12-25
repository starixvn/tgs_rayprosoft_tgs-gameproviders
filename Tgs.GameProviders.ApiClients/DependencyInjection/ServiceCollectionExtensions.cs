using Tgs.GameProviders.ApiClients.BetConstruct;
using Tgs.GameProviders.ApiClients.BigGaming;
using Tgs.GameProviders.ApiClients.TCG;
using Tgs.GameProviders.Core.BetConstruct;
using Tgs.GameProviders.Core.BigGaming;
using Tgs.GameProviders.Core.TCG;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiClients(this IServiceCollection services)
        => services.AddHttpClient<IBigGamingApiClient, BigGamingApiClient>()
        .Services
        .AddHttpClient<ITCGApiClient, TCGApiClient>()
        .Services
        .AddScoped<IBigGamingRoundDetailAnalyzeService, BigGamingRoundDetailAnalyzeService>()
        .AddScoped<IBetConstructUtility, BetConstructUtility>();
}
