using Tgs.GameCommand.Client;
using Tgs.GameProviders;
using Tgs.GameProviders.Core;
using Tgs.GameRecord.Endorphina.Session.Client;
using Tgs.GameToken.Client;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddGameProviders(
		this IServiceCollection services,
		Action<RedisOptions, IServiceProvider> configure,
		Action<IPAddressOptions, IServiceProvider> ipAddressOptions,
		Action<GameTokenClientOptions, IServiceProvider> gameTokenClientOptions,
		Action<GameCommandClientOptions, IServiceProvider> gameCommandClientOptions,
		Action<GameRecordEndorphinaSessionClientOptions, IServiceProvider> gameRecordEndorphinaSessionClientOptions)
		=> services
		.AddOptions<RedisOptions>()
		.Configure(configure)
		.Services
		.AddSingleton<RedisConnectionFactory>()
		.AddGameProvidersRepositories();
}
