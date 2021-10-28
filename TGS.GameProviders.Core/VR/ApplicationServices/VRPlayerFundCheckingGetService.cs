using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using TGS.GameProviders.ApiClients;
using TGS.GameProviders.ApiClients.VR.Models;

namespace TGS.GameProviders.Core.VR.ApplicationServices
{
    public class VRPlayerFundCheckingGetService : IGetService<VRPlayerFundCheckingQuery, PlayerFundCheckingResult>
    {
        private readonly IVRApiClient _client;
        private readonly ILogger<VRPlayerFundCheckingGetService> _logger;
        private readonly IVRService _service;

        public VRPlayerFundCheckingGetService(
            ILogger<VRPlayerFundCheckingGetService> logger,
            IVRApiClient client,
            IVRService service)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async ValueTask<PlayerFundCheckingResult> GetAsync(VRPlayerFundCheckingQuery command, CancellationToken cancellationToken = default)
        {
            var brandMerchant = await _service.GetBrandMerchantAsync(command.BrandCode).ConfigureAwait(false);

            if (brandMerchant == null)
            {
                return null;
            }

            var data = new VRFundCheckingRequest
            {
                StartTime = command.StartTime.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                EndTime = command.EndTime.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                SerialNumber = command.OrderId,
                PlayerCode = command.PlayerCode,
                RecordPage = 0,
                RecordCountPerPage = 1
            };

            var response = await _client.FundCheckingAsync(
                brandMerchant.ApiUrl,
                brandMerchant.Merchantid,
                brandMerchant.Version,
                brandMerchant.Deskey,
                data)
                .ConfigureAwait(false);

            return new() { OrderStatus = (response != null && response?.TotalRecords == 1) };
        }
    }
}
