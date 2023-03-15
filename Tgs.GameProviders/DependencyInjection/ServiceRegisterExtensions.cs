using Tgs.GameCommand.Client;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.AwcService.Client;
using Tgs.GameInterface.FaChai.Client;
using Tgs.GameProviders.Core;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceRegisterExtensions
{
    public static async Task<WebApplication> UseRegister(this WebApplication app)
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
            try
            {
                var gameInterface = scope.ServiceProvider.GetRequiredService<IGameInterfaceAwcServiceClient>();
                _ = await gameInterface
                     .CheckingServiceAsync(
                        machineName: Environment.MachineName)
                     .ConfigureAwait(false);
            }
            catch
            {

            }

            try
            {
                var gameInterface = scope.ServiceProvider.GetRequiredService<IGameInterfaceFaChaiClient>();
                _ = await gameInterface
                     .CheckingServiceAsync(
                        machineName: Environment.MachineName)
                     .ConfigureAwait(false);
            }
            catch
            {

            }
        }

        return app;
    }
}
