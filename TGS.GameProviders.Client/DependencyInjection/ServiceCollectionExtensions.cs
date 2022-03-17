using System;
using Microsoft.Extensions.Options;
using TGS.GameProviders.Client;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddGameProvidersClient(
            this IServiceCollection services,
            Action<GameProvidersClientOptions, IServiceProvider> configure)
            =>
            services
            .AddOptions<GameProvidersClientOptions>()
            .Configure(configure)
            .Services
            .AddHttpClient<IGameProvidersClient, GameProvidersClient>()
            .ConfigureHttpClient((sp, httpClient) =>
            {
                var options = sp.GetRequiredService<IOptions<GameProvidersClientOptions>>().Value;
                httpClient.BaseAddress = options.BaseUrl;
            })
            .Services
            .AddHttpClient<IGameRecordListClient, GameRecordListClient>()
            .ConfigureHttpClient((sp, httpClient) =>
            {
                var options = sp.GetRequiredService<IOptions<GameProvidersClientOptions>>().Value;
                httpClient.BaseAddress = options.BaseUrl;
            })
            .Services
            .AddHttpClient<IDreamGameClient, DreamGameClient>()
            .ConfigureHttpClient((sp, httpClient) =>
            {
                var options = sp.GetRequiredService<IOptions<GameProvidersClientOptions>>().Value;
                httpClient.BaseAddress = options.BaseUrl;
            })
            .Services;
    }
}
