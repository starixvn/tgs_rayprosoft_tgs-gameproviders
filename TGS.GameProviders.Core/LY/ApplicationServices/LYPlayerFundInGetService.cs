using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core.LY.Models;

namespace TGS.GameProviders.Core.LY.ApplicationServices
{
    public class LYPlayerFundInGetService : IGetService<LYPlayerFundInQuery, PlayerFundResult>
    {
        private readonly ILYApiClient _client;
        private readonly ILogger<LYPlayerFundInGetService> _logger;
        private readonly ILYService _service;

        public LYPlayerFundInGetService(
            ILogger<LYPlayerFundInGetService> logger,
            ILYApiClient client,
            ILYService service)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async ValueTask<PlayerFundResult> GetAsync(LYPlayerFundInQuery command, CancellationToken cancellationToken = default)
        {
            var brandMerchant = await _service.GetBrandMerchantAsync(command.BrandCode).ConfigureAwait(false);

            if (brandMerchant == null)
            {
                return null;
            }
            var agentId = await _service.GetAgentIdAsync(command.BrandCode, command.CurrencyCode).ConfigureAwait(false);

            var dateTimeOffset = _service.GetDateTimeOffsetTimeZone8();
            var orderId = $"{agentId}{dateTimeOffset:yyyyMMddHHmmssfff}{command.PlayerCode}";

            var data = new LYFnudRequest
            {
                PlayerCode = command.PlayerCode,
                Amount = command.Amount,
                OrderId = orderId
            };

            var response = await _client.FundAsync(
                brandMerchant.ApiUrl,
                brandMerchant.Deskey,
                brandMerchant.Md5key,
                agentId, dateTimeOffset,
                LYAction.FundIn,
                data)
                .ConfigureAwait(false);

            return (response != null && response?.Data.Code == 0) ? new() { OrderId = orderId } : null;
        }
    }
}
