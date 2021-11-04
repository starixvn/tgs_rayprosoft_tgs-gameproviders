using System;
using Microsoft.Extensions.DependencyInjection;
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
            .AddHttpClient<GameProvidersClient, GameProvidersClient>(
                (httpClient, sp) => ActivatorUtilities.CreateInstance<GameProvidersClient>(
                    sp,
                    httpClient,
                    sp.GetRequiredService<IOptions<GameProvidersClientOptions>>().Value.JsonSerializerOptions!))
            .ConfigureHttpClient((sp, httpClient) =>
            {
                var options = sp.GetRequiredService<IOptions<GameProvidersClientOptions>>().Value;
                httpClient.BaseAddress = options.BaseUrl;
            })
            .Services
            .AddHttpClient<GameRecordListClient, GameRecordListClient>(
                (httpClient, sp) => ActivatorUtilities.CreateInstance<GameRecordListClient>(
                    sp,
                    httpClient,
                    sp.GetRequiredService<IOptions<GameProvidersClientOptions>>().Value.JsonSerializerOptions!))
            .ConfigureHttpClient((sp, httpClient) =>
            {
                var options = sp.GetRequiredService<IOptions<GameProvidersClientOptions>>().Value;
                httpClient.BaseAddress = options.BaseUrl;
            })
            .Services
            .AddTransient<IGameProvidersClient>(sp => sp.GetRequiredService<GameProvidersClient>())
            .AddTransient<IGameRecordListClient>(sp => sp.GetRequiredService<GameRecordListClient>());
    }
}
