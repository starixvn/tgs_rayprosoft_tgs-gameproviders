using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.ApiClients;
using TGS.GameProviders.ApiClients.VR;
using TGS.GameProviders.ApiClients.VR.Models;
using TGS.ServiceUtility;

namespace TGS.GameProviders.Core.VR.ApplicationServices
{
    public class VRPlayerFundOutGetService : IGetService<VRPlayerFundOutQuery, PlayerFundResult>
    {
        private readonly IVRApiClient _client;
        private readonly ILogger<VRPlayerFundOutGetService> _logger;
        private readonly IVRService _service;

        public VRPlayerFundOutGetService(
            ILogger<VRPlayerFundOutGetService> logger,
            IVRApiClient client,
            IVRService service)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async ValueTask<PlayerFundResult> GetAsync(VRPlayerFundOutQuery command, CancellationToken cancellationToken = default)
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
                Type = 0,
                Amount = command.Amount,
                CreateTime = date.ToString("yyyy-MM-ddTHH:mm:ssZ")
            };

            var response = await _client.FundAsync(
                brandMerchant.ApiUrl,
                brandMerchant.Merchantid,
                brandMerchant.Version,
                brandMerchant.Deskey,
                VRAction.FundOut,
                data).ConfigureAwait(false);

            return (response != null && response?.State == 0) ? new() { OrderId = orderId } : null;
        }
    }
}
