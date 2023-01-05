using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.PragmaticPlayTransfer.ApplicationServices;

internal class PragmaticPlayTransferGameUrlGetService : IGetService<PragmaticPlayTransferGameUrlQuery, GameUrlResult>
{
    private readonly IPragmaticPlayTransferApiClient _client;
    private readonly IGameRepository _gameRepository;
    private readonly IPlayerRepository _playerService;
    private readonly IPragmaticPlayTransferGamePlayer _pragmaticPlayTransferGamePlayer;
    private readonly IPragmaticPlayTransferService _service;

    public PragmaticPlayTransferGameUrlGetService(
        IPlayerRepository playerService,
        IGameRepository gameRepository,
        IPragmaticPlayTransferService service,
        IPragmaticPlayTransferApiClient client,
        IPragmaticPlayTransferGamePlayer pragmaticPlayTransferGamePlayer)
    {
        _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
        _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _pragmaticPlayTransferGamePlayer = pragmaticPlayTransferGamePlayer ?? throw new ArgumentNullException(nameof(pragmaticPlayTransferGamePlayer));
    }

    public async ValueTask<GameUrlResult> GetAsync(
        PragmaticPlayTransferGameUrlQuery query,
        CancellationToken cancellationToken = default)
    {
        var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);
        if (configuration is null)
            return null;

        var game = await _gameRepository.GetAsync(query.GameId!.Value).ConfigureAwait(false);
        if (game is null)
            return null;

        var culture = await _service.GetGameProviderLanguageCodeAsync(game.GameProviderId, query.Language).ConfigureAwait(false);

        var platform = query.PlatformType == PlatformType.Mobile ? "MOBILE" : "WEB";

        var (isPlayerExist, url) = await _client
            .GetGameUrlAsync(
                configuration.ApiUrl,
                configuration.SecureLogin,
                configuration.SecretKey,
                query.PlayerCode,
                game.GameExternalId,
                culture,
                platform,
                query.BackUrl?.OriginalString)
            .ConfigureAwait(false);

        if (isPlayerExist)
        {
            return new GameUrlResult
            {
                Url = new Uri(url)
            };
        }

        var playerInfo = await _playerService.GetAsync(query.PlayerCode).ConfigureAwait(false);
        var gameProviderId = await _service.GetGameProviderIdAsync().ConfigureAwait(false);
        var gameProviderCurrencyCode = await _service
            .GetGameProviderCurrencyCodeAsync(gameProviderId, playerInfo.DefaultWalletCurrencyCode)
            .ConfigureAwait(false);

        if (string.IsNullOrEmpty(gameProviderCurrencyCode))
        {
            throw new Exception($"Cannot find matched {GameProviderType.PragmaticPlayTransfer} currency of '{playerInfo.DefaultWalletCurrencyCode}'");
        }

        var created = await _pragmaticPlayTransferGamePlayer
            .CreatePlayerAsync(query.PlayerCode, gameProviderCurrencyCode, configuration)
            .ConfigureAwait(false);

        if (created)
        {
            (isPlayerExist, url) = await _client
                .GetGameUrlAsync(
                    configuration.ApiUrl,
                    configuration.SecureLogin,
                    configuration.SecretKey,
                    query.PlayerCode,
                    game.GameExternalId,
                    culture,
                    platform,
                    query.BackUrl?.OriginalString)
                .ConfigureAwait(false);
        }

        return !isPlayerExist
            ? throw new Exception($"Failed to create this player.({query.PlayerCode})")
            : new GameUrlResult
            {
                Url = new Uri(url)
            };
    }
}
