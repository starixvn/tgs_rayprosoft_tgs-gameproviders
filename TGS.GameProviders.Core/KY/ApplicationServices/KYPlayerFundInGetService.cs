using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.KY.ApplicationServices;

public class KYPlayerFundInGetService : IGetService<KYPlayerFundInQuery, PlayerFundResult>
{
    private readonly IKYApiClient _client;
    private readonly ILogger<KYPlayerFundInGetService> _logger;
    private readonly IKYService _service;

    public KYPlayerFundInGetService(
        ILogger<KYPlayerFundInGetService> logger,
        IKYApiClient client,
        IKYService service)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    public async ValueTask<PlayerFundResult> GetAsync(KYPlayerFundInQuery command, CancellationToken cancellationToken = default)
    {
        var brandMerchant = await _service.GetBrandMerchantAsync(command.BrandCode).ConfigureAwait(false);

        if (brandMerchant == null)
        {
            return null;
        }
        var agentId = await _service.GetAgentIdAsync(command.BrandCode, command.CurrencyCode).ConfigureAwait(false);

        var (Success, orderId) = await _client.FundInAsync(
            brandMerchant.ApiUrl,
            brandMerchant.Deskey,
            brandMerchant.Md5key,
            agentId,
            command.PlayerCode,
            command.Amount
            ).ConfigureAwait(false);

        return (Success) ? new() { OrderId = orderId } : null;
    }
}
