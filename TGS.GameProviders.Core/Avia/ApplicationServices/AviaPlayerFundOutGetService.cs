using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;

namespace TGS.GameProviders.Core.Avia.ApplicationServices
{
    public class AviaPlayerFundOutGetService : IGetService<AviaPlayerFundInQuery, PlayerFundResult>
    {
        private readonly IAviaApiClient _client;
        private readonly ILogger<AviaPlayerFundOutGetService> _logger;
        private readonly IAviaService _service;

        public AviaPlayerFundOutGetService(
            ILogger<AviaPlayerFundOutGetService> logger,
            IAviaApiClient client,
            IAviaService service)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async ValueTask<PlayerFundResult> GetAsync(AviaPlayerFundInQuery command, CancellationToken cancellationToken = default)
        {
            var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);
            var gameProviderCurrencyCode = await _service.GetGameProviderCurrencyCodeAsync(command.CurrencyCode).ConfigureAwait(false);
            var (Success, orderId) = await _client.TransferOutAsync(
                configuration.ApiUrl,
                configuration.Authorization,
                command.PlayerCode,
                gameProviderCurrencyCode,
                command.OrderId,
                command.Amount
                ).ConfigureAwait(false);

            return (Success) ? new() { OrderId = orderId } : null;
        }
    }
}
