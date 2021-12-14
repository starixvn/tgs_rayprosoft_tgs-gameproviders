using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;

namespace TGS.GameProviders.Core.Avia.ApplicationServices
{
    public class AviaPlayerFundCheckingGetService : IGetService<AviaPlayerFundCheckingQuery, PlayerFundCheckingResult>
    {
        private readonly IAviaApiClient _client;
        private readonly ILogger<AviaPlayerFundCheckingGetService> _logger;
        private readonly IAviaService _service;

        public AviaPlayerFundCheckingGetService(
            ILogger<AviaPlayerFundCheckingGetService> logger,
            IAviaApiClient client,
            IAviaService service)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async ValueTask<PlayerFundCheckingResult> GetAsync(AviaPlayerFundCheckingQuery command, CancellationToken cancellationToken = default)
        {
            var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);

            var response = await _client.TranslateinfoAsync(
                configuration.ApiUrl,
                configuration.Authorization,
                command.OrderId).ConfigureAwait(false);

            return new() { OrderStatus = response };
        }
    }
}
