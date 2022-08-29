using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.PragmaticPlayTransfer.ApplicationServices;

internal class PragmaticPlayTransferPlayerBalanceGetService : IGetService<PragmaticPlayTransferPlayerBalanceQuery, decimal?>
{
    private readonly IPragmaticPlayTransferApiClient _client;
    private readonly IPragmaticPlayTransferGamePlayer _gamePlayer;
    private readonly IPlayerRepository _playerService;
    private readonly IPragmaticPlayTransferService _service;

    public PragmaticPlayTransferPlayerBalanceGetService(
        IPlayerRepository playerService,
        IPragmaticPlayTransferService service,
        IPragmaticPlayTransferGamePlayer gamePlayer,
        IPragmaticPlayTransferApiClient client)
    {
        _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _gamePlayer = gamePlayer ?? throw new ArgumentNullException(nameof(gamePlayer));
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask<decimal?> GetAsync(
        PragmaticPlayTransferPlayerBalanceQuery query,
        CancellationToken cancellationToken = default)
    {
        var playerInfo = await _playerService.GetAsync(query.PlayerCode).ConfigureAwait(false);
        if (playerInfo is null)
        {
            throw new Exception($"The player does not exist.{query.PlayerCode}");
        }

        var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);
        if (configuration is null)
        {
            throw new Exception($"The {GameProviderType.PragmaticPlayTransfer} configuration does not exist.");
        }

        var (isPlayerExist, balance) = await _client.GetBalanceAsync(
                configuration.ApiUrl,
                configuration.SecureLogin,
                configuration.SecretKey,
                query.PlayerCode)
            .ConfigureAwait(false);

        if (isPlayerExist is true)
            return balance;

        var gameProviderId = await _service.GetGameProviderIdAsync().ConfigureAwait(false);
        var gameProviderCurrencyCode = await _service
            .GetGameProviderCurrencyCodeAsync(gameProviderId, playerInfo.DefaultWalletCurrencyCode)
            .ConfigureAwait(false);

        if (string.IsNullOrEmpty(gameProviderCurrencyCode))
        {
            throw new Exception($"Cannot find matched {GameProviderType.PragmaticPlayTransfer} currency of '{playerInfo.DefaultWalletCurrencyCode}'");
        }

        var created = await _gamePlayer
            .CreatePlayerAsync(query.PlayerCode, gameProviderCurrencyCode, configuration)
            .ConfigureAwait(false);

        if (created)
        {
            (_, balance) = await _client.GetBalanceAsync(
                    configuration.ApiUrl,
                    configuration.SecureLogin,
                    configuration.SecretKey,
                    query.PlayerCode)
                .ConfigureAwait(false);
        }

        return balance;
    }
}
