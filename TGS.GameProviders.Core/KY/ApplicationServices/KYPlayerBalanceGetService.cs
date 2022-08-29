using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.KY.ApplicationServices;

public class KYPlayerBalanceGetService : IGetService<KYPlayerBalanceQuery, decimal?>
{
    private readonly IKYApiClient _client;
    private readonly ILogger<KYPlayerBalanceGetService> _logger;
    private readonly IPlayerRepository _playerService;
    private readonly IKYService _service;

    public KYPlayerBalanceGetService(
        ILogger<KYPlayerBalanceGetService> logger,
        IKYApiClient client,
        IKYService service,
        IPlayerRepository playerService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
    }

    public async ValueTask<decimal?> GetAsync(KYPlayerBalanceQuery command, CancellationToken cancellationToken = default)
    {
        var playerModel = await _playerService.GetAsync(command.PlayerCode).ConfigureAwait(false);
        var brandMerchant = await _service.GetBrandMerchantAsync(playerModel.BrandCode).ConfigureAwait(false);
        var agentId = await _service.GetAgentIdAsync(playerModel.BrandCode, playerModel.DefaultWalletCurrencyCode).ConfigureAwait(false);

        return await _client.GetBalanceAsync(brandMerchant.ApiUrl, brandMerchant.Deskey, brandMerchant.Md5key, agentId, playerModel.PlayerCode).ConfigureAwait(false);
    }
}
