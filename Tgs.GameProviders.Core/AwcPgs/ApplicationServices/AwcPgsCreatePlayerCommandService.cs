using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.AwcService.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.AwcPgs.ApplicationServices;

internal class AwcPgsCreatePlayerCommandService : ICommandService<AwcPgsCreatePlayerCommand>
{
    private readonly IGameInterfaceAwcServiceClient _client;
    private readonly IGameProviderRepository _gameProvider;

    public AwcPgsCreatePlayerCommandService(
        IGameProviderRepository gameProvider,
        IGameInterfaceAwcServiceClient client)
    {
        _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask ExecuteAsync(AwcPgsCreatePlayerCommand command, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _gameProvider.GetGameProviderIdAsync(GameProviderType.AWC_PGS).ConfigureAwait(false);
        var cultureCode = await _gameProvider.GetGameProviderCultureCodeAsync(gameProviderId, command.Language).ConfigureAwait(false) ?? "en";
        var currency = await _gameProvider.GetGameProviderCurrencyCodeAsync(gameProviderId, command.CurrencyCode).ConfigureAwait(false);

        await _client.CreatePlayerAsync(
            apiUrl: command.Configuration.ApiUrl,
            agentId: command.ConfigurationData.AgentId,
            cert: command.ConfigurationData.Cert,
            playerCode: command.PlayerCode,
            userName: command.PlayerCode,
            currency: currency,
            language: cultureCode,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}
