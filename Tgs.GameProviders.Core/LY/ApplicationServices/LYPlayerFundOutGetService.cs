using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.LY.ApplicationServices;

public class LYPlayerFundOutGetService : IGetService<LYPlayerFundOutQuery, PlayerFundResult>
{
    private readonly ILYApiClient _client;
    private readonly ILogger<LYPlayerFundOutGetService> _logger;
    private readonly ILYService _service;

    public LYPlayerFundOutGetService(
        ILogger<LYPlayerFundOutGetService> logger,
        ILYApiClient client,
        ILYService service)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    public async ValueTask<PlayerFundResult> GetAsync(LYPlayerFundOutQuery command, CancellationToken cancellationToken = default)
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
