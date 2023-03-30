using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.SkywindService.Client;
using Tgs.GameInterface.SkywindService.Protos;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.SkywindTransfer.ApplicationServices;

internal class SkywindTransferCreatePlayerCommandService : ICommandService<SkywindTransferCreatePlayerCommand>
{
    private readonly IGameInterfaceSkywindClient _client;
    private readonly IGameProviderRepository _gameProvider;

    public SkywindTransferCreatePlayerCommandService(
        IGameInterfaceSkywindClient client,
        IGameProviderRepository gameProvider)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
    }

    public async ValueTask ExecuteAsync(SkywindTransferCreatePlayerCommand command, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _gameProvider.GetGameProviderIdAsync(GameProviderType.SkywindTransfer).ConfigureAwait(false);

        var gameProviderCurrencyCode = await _gameProvider
                .GetGameProviderCurrencyCodeAsync(gameProviderId, command.CurrencyCode)
                .ConfigureAwait(false);

        var request = new PlayerCreateRequest()
        {
            ApiUrl = command.Configuration.ApiUrl.ToString(),
            SecretKey = command.Configuration.SecretKey,
            Username = command.Configuration.Username,
            Password = command.Configuration.Password,
            PlayerCode = command.PlayerCode,
            Currency = gameProviderCurrencyCode,
        };
        _ = await _client
                    .PlayerCreateAsync(
                        request: request,
                        cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
    }
}
