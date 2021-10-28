using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.ApiClients;
using TGS.GameProviders.ApiClients.WGHorse.Models;
using TGS.ServiceUtility;

namespace TGS.GameProviders.Core.WGHorse.ApplicationServices
{
    public class WGHorsePlayerFundCheckingGetService : IGetService<WGHorsePlayerFundCheckingQuery, PlayerFundCheckingResult>
    {
        private readonly IWGHorseApiClient _client;
        private readonly ILogger<WGHorsePlayerFundCheckingGetService> _logger;
        private readonly IWGHorseService _service;

        public WGHorsePlayerFundCheckingGetService(
            ILogger<WGHorsePlayerFundCheckingGetService> logger,
            IWGHorseApiClient client,
            IWGHorseService service)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async ValueTask<PlayerFundCheckingResult> GetAsync(WGHorsePlayerFundCheckingQuery command, CancellationToken cancellationToken = default)
        {
            var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);

            var data = new WGHorseAccessMoneyQueryRequest
            {
                OrderId = command.OrderId
            };

            var response = await _client.AccessMoneyQueryAsync(
                configuration.ApiUrl,
                configuration.PartnerCode,
                configuration.PartnerKey,
                configuration.IV,
                data)
                .ConfigureAwait(false);

            return new() { OrderStatus = (response != null && response?.ReturnCode == 1) };
        }
    }
}
