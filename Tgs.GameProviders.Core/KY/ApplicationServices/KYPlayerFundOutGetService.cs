using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.KY.ApplicationServices;

public class KYPlayerFundOutGetService : IGetService<KYPlayerFundOutQuery, PlayerFundResult>
{
    private readonly IKYApiClient _client;
    private readonly ILogger<KYPlayerFundOutGetService> _logger;
    private readonly IKYService _service;

    public KYPlayerFundOutGetService(
        ILogger<KYPlayerFundOutGetService> logger,
        IKYApiClient client,
        IKYService service)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    public async ValueTask<PlayerFundResult> GetAsync(KYPlayerFundOutQuery command, CancellationToken cancellationToken = default)
    {
        var brandMerchant = await _service.GetBrandMerchantAsync(command.BrandCode).ConfigureAwait(false);

        if (brandMerchant == null)
        {
            return null;
        }

        var agentId = await _service.GetAgentIdAsync(command.BrandCode, command.CurrencyCode).ConfigureAwait(false);

        var (Success, orderId) = await _client.FundOutAsync(
            brandMerchant.ApiUrl,
            brandMerchant.Deskey,
            brandMerchant.Md5key,
            agentId,
            command.PlayerCode,
            command.Amount).ConfigureAwait(false);

        return (Success) ? new() { OrderId = orderId } : null;
    }
}
