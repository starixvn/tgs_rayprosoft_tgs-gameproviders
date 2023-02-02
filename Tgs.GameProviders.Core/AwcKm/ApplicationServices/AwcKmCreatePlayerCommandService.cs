using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.AwcService.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.AwcKm.ApplicationServices;

internal class AwcKmCreatePlayerCommandService : ICommandService<AwcKmCreatePlayerCommand>
{
    private readonly IGameInterfaceAwcServiceClient _client;
    private readonly IGameProviderRepository _gameProvider;

    public AwcKmCreatePlayerCommandService(
        IGameProviderRepository gameProvider,
        IGameInterfaceAwcServiceClient client)
    {
        _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask ExecuteAsync(AwcKmCreatePlayerCommand command, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _gameProvider.GetGameProviderIdAsync(GameProviderType.AWC_KM).ConfigureAwait(false);
        var cultureCode = await _gameProvider.GetGameProviderCultureCodeAsync(gameProviderId, command.Language).ConfigureAwait(false) ?? "en";
        var currency = await _gameProvider.GetGameProviderCurrencyCodeAsync(gameProviderId, command.CurrencyCode).ConfigureAwait(false)
            ?? throw new Exception($"This game product currency does not exist.({command.CurrencyCode})");

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
