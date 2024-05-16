using Microsoft.Extensions.Options;
using Tgs.GameProviders.Client;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddGameProvidersClient(
		this IServiceCollection services,
		Action<GameProvidersClientOptions, IServiceProvider> configure)
		=> services
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
		.AddHttpClient<IGameProvidersBalancesClient, GameProvidersBalancesClient>()
		.ConfigureHttpClient((sp, httpClient) =>
		{
			var options = sp.GetRequiredService<IOptions<GameProvidersClientOptions>>().Value;
			httpClient.BaseAddress = options.BaseUrl;
		})
		.Services;
}
