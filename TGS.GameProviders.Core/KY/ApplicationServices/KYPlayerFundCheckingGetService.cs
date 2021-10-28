using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.ApiClients;
using TGS.GameProviders.ApiClients.KY.Models;
using TGS.ServiceUtility;

namespace TGS.GameProviders.Core.KY.ApplicationServices
{
    public class KYPlayerFundCheckingGetService : IGetService<KYPlayerFundCheckingQuery, PlayerFundCheckingResult>
    {
        private readonly IKYApiClient _client;
        private readonly ILogger<KYPlayerFundCheckingGetService> _logger;
        private readonly IKYService _service;

        public KYPlayerFundCheckingGetService(
            ILogger<KYPlayerFundCheckingGetService> logger,
            IKYApiClient client,
            IKYService service)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async ValueTask<PlayerFundCheckingResult> GetAsync(KYPlayerFundCheckingQuery command, CancellationToken cancellationToken = default)
        {
            var brandMerchant = await _service.GetBrandMerchantAsync(command.BrandCode).ConfigureAwait(false);

            if (brandMerchant == null)
            {
                return null;
            }
            var agentId = await _service.GetAgentIdAsync(command.BrandCode, command.CurrencyCode).ConfigureAwait(false);

            var dateTimeOffset = _service.GetDateTimeOffsetTimeZone8();

            var data = new KYFundCheckingRequest
            {
                OrderId = command.OrderId
            };

            var response = await _client.FundCheckingAsync(
                brandMerchant.ApiUrl,
                brandMerchant.Deskey,
                brandMerchant.Md5key,
                agentId,
                data)
                .ConfigureAwait(false);

            return new() { OrderStatus = (response != null && response?.Data.Code == 0 && response?.Data.Status == 0) };
        }
    }
}
