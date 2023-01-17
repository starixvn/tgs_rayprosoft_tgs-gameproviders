using Tgs.GameCommand.Client;
using Tgs.GameCommand.Common;
using Tgs.GameProviders.Core;

namespace Microsoft.Extensions.DependencyInjection;

public static class GameProvidersRegisterExtensions
{
    public static async Task<WebApplication> UseGameProvidersRegister(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var client = scope.ServiceProvider.GetRequiredService<IGameCommandClient>();

            var gameProviderRepository = scope.ServiceProvider.GetRequiredService<IGameProviderRepository>();

            var gameProviderIdInfos = client
                .GameProviderGetGameProviderIdsAsync()
                .ToEnumerable()
                ?? Enumerable.Empty<GameProviderIdInfo>();

            foreach (var gameProviderIdInfo in gameProviderIdInfos)
            {
                _ = await gameProviderRepository
                    .GetGameProviderIdAsync(
                        gameProviderCode: gameProviderIdInfo.GameProviderCode)
                    .ConfigureAwait(false);

                _ = await gameProviderRepository
                    .GetAsync(
                        gameProviderId: gameProviderIdInfo.GameProviderId)
                    .ConfigureAwait(false);
            }
        }

        return app;
    }
}
