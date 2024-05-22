using Tgs.GameCommand.Client;
using Tgs.GameCommand.Common;
using Tgs.GameProviders.Core;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceRegisterExtensions
{
	public static async Task<WebApplication> InitialMemory(this WebApplication app)
	{
		using (var scope = app.Services.CreateScope())
		{
			var client = scope.ServiceProvider.GetRequiredService<IGameCommandClient>();

			var gameProviderRepository = scope.ServiceProvider.GetRequiredService<IGameProviderRepository>();

			var gameProviderInfos = client
				.GameProviderQueryAsync()
				.ToEnumerable()
				?? Enumerable.Empty<GameProviderInfo>();

			foreach (var gameProviderInfo in gameProviderInfos)
			{
				_ = await gameProviderRepository
					.GetGameProviderIdAsync(
						gameProviderCode: gameProviderInfo.GameProviderCode)
					.ConfigureAwait(false);

				_ = await gameProviderRepository
					.GetAsync(
						gameProviderId: gameProviderInfo.GameProviderId)
					.ConfigureAwait(false);
			}
		}

		return app;
	}
}
