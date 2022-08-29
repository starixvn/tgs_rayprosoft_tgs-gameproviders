using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.Avia.ApplicationServices;

public class AviaPlayerBalanceGetService : IGetService<AviaPlayerBalanceQuery, decimal?>
{
    private readonly IAviaApiClient _client;
    private readonly IAviaGamePlayerService _gamePlayer;
    private readonly ILogger<AviaPlayerBalanceGetService> _logger;
    private readonly IPlayerRepository _playerService;
    private readonly IAviaService _service;

    public AviaPlayerBalanceGetService(
        ILogger<AviaPlayerBalanceGetService> logger,
        IAviaApiClient client,
        IAviaService service,
        IPlayerRepository playerService,
        IAviaGamePlayerService gamePlayer)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
        _gamePlayer = gamePlayer ?? throw new ArgumentNullException(nameof(gamePlayer));
    }

    public async ValueTask<decimal?> GetAsync(AviaPlayerBalanceQuery command, CancellationToken cancellationToken = default)
    {
        var playerModel = await _playerService.GetAsync(command.PlayerCode).ConfigureAwait(false);
        var gameProviderCurrencyCode = await _service.GetGameProviderCurrencyCodeAsync(playerModel.DefaultWalletCurrencyCode).ConfigureAwait(false);

        var brandMerchant = await _service.GetBrandMerchantAsync(playerModel.BrandCode);

        if (brandMerchant == null)
        {
            throw new ArgumentNullException(nameof(brandMerchant));
        }

        (bool IsPlayer, decimal? Balance) = await _client.BalanceAsync(
            brandMerchant.ApiUrl,
            brandMerchant.Authorization,
            playerModel.PlayerCode).ConfigureAwait(false);
        if (!IsPlayer)
        {
            var createPlayerResult = await _gamePlayer.CreatePlayerAsync(
                brandMerchant,
                command.PlayerCode,
                gameProviderCurrencyCode).ConfigureAwait(false);
            if (createPlayerResult)
            {
                (IsPlayer, Balance) = await _client.BalanceAsync(
                    brandMerchant.ApiUrl,
                    brandMerchant.Authorization,
                    playerModel.PlayerCode).ConfigureAwait(false);
            }
        }
        return Balance;
    }
}
