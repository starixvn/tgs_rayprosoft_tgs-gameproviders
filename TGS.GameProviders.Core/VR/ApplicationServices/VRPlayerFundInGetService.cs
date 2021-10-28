using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using TGS.GameProviders.ApiClients;
using TGS.GameProviders.ApiClients.VR;
using TGS.GameProviders.ApiClients.VR.Models;

namespace TGS.GameProviders.Core.VR.ApplicationServices
{
    public class VRPlayerFundInGetService : IGetService<VRPlayerFundInQuery, PlayerFundResult>
    {
        private readonly IVRApiClient _client;
        private readonly ILogger<VRPlayerFundInGetService> _logger;
        private readonly IVRService _service;

        public VRPlayerFundInGetService(
            ILogger<VRPlayerFundInGetService> logger,
            IVRApiClient client,
            IVRService service)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async ValueTask<PlayerFundResult> GetAsync(VRPlayerFundInQuery command, CancellationToken cancellationToken = default)
        {
            var brandMerchant = await _service.GetBrandMerchantAsync(command.BrandCode).ConfigureAwait(false);
            if (brandMerchant == null)
            {
                return null;
            }

            var orderId = command.OrderId;
            var date = DateTimeOffset.UtcNow;
            var data = new VRFnudRequest()
            {
                SerialNumber = orderId,
                PlayerCode = command.PlayerCode,
                Type = (int)VRAmountTypes.FundIn,
                Amount = command.Amount,
                CreateTime = date.ToString("yyyy-MM-ddTHH:mm:ssZ")
            };

            var response = await _client.FundAsync(
                brandMerchant.ApiUrl,
                brandMerchant.Merchantid,
                brandMerchant.Version,
                brandMerchant.Deskey,
                VRAction.FundIn,
                data).ConfigureAwait(false);

            return (response != null && response?.State == 0) ? new() { OrderId = orderId } : null;
        }
    }
}
