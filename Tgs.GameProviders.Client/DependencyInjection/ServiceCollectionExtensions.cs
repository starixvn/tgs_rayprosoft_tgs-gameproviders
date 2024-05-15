using Tgs.GameProviders.Client;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddGameProvidersClient(
		this IServiceCollection services, Action<IHttpClientBuilder> httpClientConfigure)
	{
		httpClientConfigure(
			services
			.AddHttpClient<IGameProvidersClient, GameProvidersClient>()
			.Services
			.AddHttpClient<IGameProvidersBalancesClient, GameProvidersBalancesClient>());

		return services;
	}
}
