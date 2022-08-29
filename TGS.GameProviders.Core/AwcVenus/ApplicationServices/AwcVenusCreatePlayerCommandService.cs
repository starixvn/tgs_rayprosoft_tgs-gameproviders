using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.AwcService.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.AwcVenus.ApplicationServices;

internal class AwcVenusCreatePlayerCommandService : ICommandService<AwcVenusCreatePlayerCommand>
{
    private readonly IGameProviderRepository _gameProvider;
    private readonly IGameInterfaceAwcServiceGrpc _grpc;

    public AwcVenusCreatePlayerCommandService(
        IGameProviderRepository gameProvider,
        IGameInterfaceAwcServiceGrpc grpc)
    {
        _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
        _grpc = grpc ?? throw new ArgumentNullException(nameof(grpc));
    }

    public async ValueTask ExecuteAsync(AwcVenusCreatePlayerCommand command, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _gameProvider.GetGameProviderIdAsync(GameProviderType.AWC_VENUS).ConfigureAwait(false);
        var cultureCode = await _gameProvider.GetGameProviderCultureCodeAsync(gameProviderId, command.Language).ConfigureAwait(false) ?? "en";
        var currency = await _gameProvider.GetGameProviderCurrencyCodeAsync(gameProviderId, command.CurrencyCode).ConfigureAwait(false)
            ?? throw new Exception($"This game product currency does not exist.({command.CurrencyCode})");

        await _grpc.CreatePlayerByBetLimitAsync(
            apiUrl: command.Configuration.ApiUrl,
            agentId: command.ConfigurationData.AgentId,
            cert: command.ConfigurationData.Cert,
            playerCode: command.PlayerCode,
            userName: command.PlayerCode,
            currency: currency,
            betLimit: JsonSerializer.Serialize(command.ConfigurationData.BetLimit),
            language: cultureCode,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}
