using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using TGS.GameProviders.ApiClients;
using TGS.GameProviders.ApiClients.VR.Models;
using TGS.GameProviders.Domains.Extensions;

namespace TGS.GameProviders.Core.VR.ApplicationServices
{
    public class VRGameRecordListGetService : IGetService<VRGameRecordListQuery, VRGameRecordListResult>
    {
        private readonly IVRApiClient _client;
        private readonly ILogger<VRGameRecordListGetService> _logger;
        private readonly IVRService _service;

        public VRGameRecordListGetService(
            IVRApiClient client,
            ILogger<VRGameRecordListGetService> logger,
            IVRService service
            )
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async ValueTask<VRGameRecordListResult> GetAsync(VRGameRecordListQuery command, CancellationToken cancellationToken = default)
        {
            var brandMerchant = await _service.GetBrandMerchantAsync(command.BrandCode).ConfigureAwait(false);
            if (brandMerchant == null)
            {
                return new()
                {
                    Success = false
                };
            }

            var data = new VRGameRecordRequest
            {
                StartTime = command.StartTime.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                EndTime = command.EndTime.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                IsUpdateTime = command.IsUpdateTime,
                Page = command.Page,
                Rows = command.Rows
            };

            var response = await _client.GetGameRecordAsync(
                brandMerchant.ApiUrl,
                brandMerchant.Merchantid,
                brandMerchant.Version,
                brandMerchant.Deskey,
                data);

            if (response == null)
            {
                return new() { Success = false };
            }

            return new() { Success = true, Datas = (response.Records.Length > 0) ? response.Records : null };
        }
    }
}