using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.ApiClients;
using TGS.GameProviders.ApiClients.VR.Models;
using TGS.ServiceUtility;

namespace TGS.GameProviders.Core.VR.ApplicationServices
{
    public class VRPlayerBalanceGetService : IGetService<VRPlayerBalanceQuery, PlayerBalanceResult>
    {
        private readonly IVRApiClient _client;
        private readonly ILogger<VRPlayerBalanceGetService> _logger;
        private readonly IVRGamePlayerCreate _playerCreate;
        private readonly IVRService _service;

        public VRPlayerBalanceGetService(
            ILogger<VRPlayerBalanceGetService> logger,
            IVRApiClient client,
            IVRService service,
            IVRGamePlayerCreate playerCreate
            )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _playerCreate = playerCreate ?? throw new ArgumentNullException(nameof(playerCreate));
        }

        public async ValueTask<PlayerBalanceResult> GetAsync(VRPlayerBalanceQuery command, CancellationToken cancellationToken = default)
        {
            var brandMerchant = await _service.GetBrandMerchantAsync(command.BrandCode).ConfigureAwait(false);

            if (brandMerchant == null)
            {
                return null;
            }

            var data = new VRGetBalanceRequest()
            {
                PlayerCode = command.PlayerCode
            };

            var response = await _client.GetBalanceAsync(
                brandMerchant.ApiUrl,
                brandMerchant.Merchantid,
                brandMerchant.Version,
                brandMerchant.Deskey,
                data).ConfigureAwait(false);

            if (string.IsNullOrEmpty(response.PlayerName) || response.Balance == -1)
            {
                if (await _playerCreate.CreatePlayerAsync(brandMerchant, command.PlayerCode).ConfigureAwait(false))
                {
                    response = await _client.GetBalanceAsync(
                        brandMerchant.ApiUrl,
                        brandMerchant.Merchantid,
                        brandMerchant.Version,
                        brandMerchant.Deskey,
                        data).ConfigureAwait(false);
                }
            }

            if (string.IsNullOrEmpty(response.PlayerName) || response.Balance == -1)
            {
                return null;
            }

            return new()
            {
                Balance = response.Balance
            };
        }
    }
}
