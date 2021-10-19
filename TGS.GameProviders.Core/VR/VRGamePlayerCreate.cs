using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.ApiClients;
using TGS.GameProviders.ApiClients.VR.Models;
using TGS.GameProviders.Repositories.Domains.Models.SettingConfiguration.VR;

namespace TGS.GameProviders.Core.VR
{
    internal class VRGamePlayerCreate : IVRGamePlayerCreate
    {
        private readonly IVRApiClient _client;
        private readonly ILogger<VRGamePlayerCreate> _logger;
        private readonly IVRService _service;

        public VRGamePlayerCreate(
            ILogger<VRGamePlayerCreate> logger,
            IVRApiClient client,
            IVRService service
            )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async ValueTask<bool> CreatePlayerAsync(VRBrandMerchant brandMerchant, string playerCode)
        {
            var data = new VRCreatePlayerRequest()
            {
                PlayerCode = playerCode
            };
            var response = await _client
                .CreatePlayerAsync(
                brandMerchant.ApiUrl,
                brandMerchant.Merchantid,
                brandMerchant.Version,
                brandMerchant.Deskey,
                data)
                .ConfigureAwait(false);

            return (response.ErrorCode == 0);
        }
    }
}
