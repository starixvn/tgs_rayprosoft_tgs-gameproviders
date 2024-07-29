using System;
using System.Reflection;
using Tgs.GameCommand.Client;
using Tgs.GameProviders.Core;
using Tgs.GameRecord.Endorphina.Session.Client;
using Tgs.GameToken.Client;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollecitonExtensions
{
	public static IServiceCollection AddGameProvidersCore(
		this IServiceCollection services,
		Action<IPAddressOptions, IServiceProvider> ipAddressOptions,
		Action<GameTokenClientOptions, IServiceProvider> gameTokenClientOptions,
		Action<GameCommandClientOptions, IServiceProvider> gameCommandClientOptions,
		Action<GameRecordEndorphinaSessionClientOptions, IServiceProvider> gameRecordEndorphinaClientOptions)
		=> services
		.AddOptions<IPAddressOptions>()
		.Configure(ipAddressOptions)
		.Services
		.RegisterAllServiceUtilityTypes(
			Assembly.GetExecutingAssembly(),
			ServiceLifetime.Transient)
		.AddGameCommandClient(gameCommandClientOptions)
		.AddGameRecordEndorphinaSessionClient(gameRecordEndorphinaClientOptions)
		.AddGameTokenClient(gameTokenClientOptions)
		//Tgs.Core.Infrastructures.Security
		.AddSecurity()
		.AddMemoryCache();
}
