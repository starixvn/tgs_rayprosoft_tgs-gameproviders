using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.WGSports.ApplicationServices
{
    public class WGSportsPlayerFundCheckingGetService : IGetService<WGSportsPlayerFundCheckingQuery, PlayerFundCheckingResult>
    {
        private readonly IWGSportsApiClient _client;
        private readonly ILogger<WGSportsPlayerFundCheckingGetService> _logger;
        private readonly IWGSportsService _service;

        public WGSportsPlayerFundCheckingGetService(
            ILogger<WGSportsPlayerFundCheckingGetService> logger,
            IWGSportsApiClient client,
            IWGSportsService service)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async ValueTask<PlayerFundCheckingResult> GetAsync(WGSportsPlayerFundCheckingQuery command, CancellationToken cancellationToken = default)
        {
            var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);

            var response = await _client.AccessMoneyQueryAsync(
                configuration.ApiUrl,
                configuration.PartnerCode,
                configuration.PartnerKey,
                configuration.IV,
                command.OrderId).ConfigureAwait(false);

            return new() { OrderStatus = response };
        }
    }
}
