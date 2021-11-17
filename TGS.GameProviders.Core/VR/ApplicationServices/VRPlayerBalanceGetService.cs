using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core.Repositories;
using TGS.GameProviders.Core.VR.Models;

namespace TGS.GameProviders.Core.VR.ApplicationServices
{
    public class VRPlayerBalanceGetService : IGetService<VRPlayerBalanceQuery, decimal?>
    {
        private readonly IVRApiClient _client;
        private readonly ILogger<VRPlayerBalanceGetService> _logger;
        private readonly IVRGamePlayer _playerCreate;
        private readonly IPlayerRepository _playerRepository;
        private readonly IVRService _service;

        public VRPlayerBalanceGetService(
            IVRApiClient client,
            ILogger<VRPlayerBalanceGetService> logger,
            IVRGamePlayer playerCreate,
            IPlayerRepository playerRepository,
            IVRService service
            )
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _playerCreate = playerCreate ?? throw new ArgumentNullException(nameof(playerCreate));
            _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async ValueTask<decimal?> GetAsync(VRPlayerBalanceQuery command, CancellationToken cancellationToken = default)
        {
            var playerModel = await _playerRepository.GetAsync(command.PlayerCode).ConfigureAwait(false);
            if (playerModel == null)
            {
                throw new Exception($"The player does not exist.{command.PlayerCode}");
            }

            var brandMerchant = await _service.GetBrandMerchantAsync(playerModel.BrandCode).ConfigureAwait(false);

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
                if (await _playerCreate.CreatePlayerAsync(brandMerchant, playerModel.PlayerCode).ConfigureAwait(false))
                {
                    response = await _client.GetBalanceAsync(
                        brandMerchant.ApiUrl,
                        brandMerchant.Merchantid,
                        brandMerchant.Version,
                        brandMerchant.Deskey,
                        data).ConfigureAwait(false);
                }
            }

            return (string.IsNullOrEmpty(response.PlayerName) || response.Balance == -1) ? null : response.Balance;
        }
    }
}
