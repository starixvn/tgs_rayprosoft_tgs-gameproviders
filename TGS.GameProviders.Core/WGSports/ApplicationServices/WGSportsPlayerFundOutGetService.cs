using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.WGSports.ApplicationServices;

public class WGSportsPlayerFundOutGetService : IGetService<WGSportsPlayerFundOutQuery, PlayerFundResult>
{
    private readonly IWGSportsApiClient _client;
    private readonly ILogger<WGSportsPlayerFundOutGetService> _logger;
    private readonly IWGSportsService _service;

    public WGSportsPlayerFundOutGetService(
        ILogger<WGSportsPlayerFundOutGetService> logger,
        IWGSportsApiClient client,
        IWGSportsService service)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    public async ValueTask<PlayerFundResult> GetAsync(WGSportsPlayerFundOutQuery command, CancellationToken cancellationToken = default)
    {
        var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);

        var response = await _client.AccessMoneyFundOutAsync(
            configuration.ApiUrl,
            configuration.PartnerCode,
            configuration.PartnerKey,
            configuration.IV,
            command.PlayerCode,
            command.OrderId,
            command.Amount).ConfigureAwait(false);

        return response ? new() { OrderId = command.OrderId } : null;
    }
}
