using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.AwcService.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.AwcYl.ApplicationServices;

internal class AwcYlCreatePlayerCommandService : ICommandService<AwcYlCreatePlayerCommand>
{
    private readonly IGameProviderRepository _gameProvider;
    private readonly IGameInterfaceAwcServiceGrpc _grpc;

    public AwcYlCreatePlayerCommandService(
        IGameProviderRepository gameProvider,
        IGameInterfaceAwcServiceGrpc grpc)
    {
        _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
        _grpc = grpc ?? throw new ArgumentNullException(nameof(grpc));
    }

    public async ValueTask ExecuteAsync(AwcYlCreatePlayerCommand command, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _gameProvider.GetGameProviderIdAsync(GameProviderType.AWC_YL).ConfigureAwait(false);
        var cultureCode = await _gameProvider.GetGameProviderCultureCodeAsync(gameProviderId, command.Language).ConfigureAwait(false) ?? "en";
        var currency = await _gameProvider.GetGameProviderCurrencyCodeAsync(gameProviderId, command.CurrencyCode).ConfigureAwait(false)
            ?? throw new Exception($"This game product currency does not exist.({command.CurrencyCode})");

        await _grpc.CreatePlayerAsync(
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
