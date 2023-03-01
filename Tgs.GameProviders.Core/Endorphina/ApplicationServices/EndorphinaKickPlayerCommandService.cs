using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.EndorphinaService.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.Endorphina.ApplicationServices;

internal class EndorphinaKickPlayerCommandService : ICommandService<EndorphinaKickPlayerCommand>
{
    private readonly IBrandRepository _brand;
    private readonly IGameInterfaceEndorphinaClient _client;
    private readonly IGameProviderRepository _gameProvider;
    private readonly IPlayerRepository _player;

    public EndorphinaKickPlayerCommandService(
        IBrandRepository brand,
        IGameProviderRepository gameProvider,
        IGameInterfaceEndorphinaClient client,
        IPlayerRepository player)
    {
        _brand = brand ?? throw new ArgumentNullException(nameof(brand));
        _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _player = player ?? throw new ArgumentNullException(nameof(player));
    }

    public async ValueTask ExecuteAsync(EndorphinaKickPlayerCommand command, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _gameProvider.GetGameProviderIdAsync(GameProviderType.Endorphina).ConfigureAwait(false);
        var player = await _player.GetAsync(command.PlayerCode).ConfigureAwait(false);
        var gameProviderConfigurationId = await _brand.GetBrandGameProviderConfigurationIdAsync(gameProviderId, player.BrandId).ConfigureAwait(false);
        var configuration = await _gameProvider.GetEndorphinaConfiguration(gameProviderConfigurationId).ConfigureAwait(false);

        await _client.PlayerKickAsync(
            apiUrl: configuration.ApiUrl,
            merchantId: configuration.MerchantId,
            secretKey: configuration.SecretKey,
            username: command.PlayerCode,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}
