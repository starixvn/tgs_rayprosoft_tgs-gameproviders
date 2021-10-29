using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using TGS.GameProviders.ApiClients;
using TGS.GameProviders.ApiClients.LY.Models;

namespace TGS.GameProviders.Core.LY.ApplicationServices
{
    public class LYPlayerFundCheckingGetService : IGetService<LYPlayerFundCheckingQuery, PlayerFundCheckingResult>
    {
        private readonly ILYApiClient _client;
        private readonly ILogger<LYPlayerFundCheckingGetService> _logger;
        private readonly ILYService _service;

        public LYPlayerFundCheckingGetService(
            ILogger<LYPlayerFundCheckingGetService> logger,
            ILYApiClient client,
            ILYService service)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async ValueTask<PlayerFundCheckingResult> GetAsync(LYPlayerFundCheckingQuery command, CancellationToken cancellationToken = default)
        {
            var brandMerchant = await _service.GetBrandMerchantAsync(command.BrandCode).ConfigureAwait(false);

            if (brandMerchant == null)
            {
                return null;
            }
            var agentId = await _service.GetAgentIdAsync(command.BrandCode, command.CurrencyCode).ConfigureAwait(false);

            var dateTimeOffset = _service.GetDateTimeOffsetTimeZone8();

            var data = new LYFundCheckingRequest
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
