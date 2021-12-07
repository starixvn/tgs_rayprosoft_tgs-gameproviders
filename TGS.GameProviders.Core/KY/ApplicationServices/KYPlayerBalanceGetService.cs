using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core.Repositories;

namespace TGS.GameProviders.Core.KY.ApplicationServices
{
    public class KYPlayerBalanceGetService : IGetService<KYPlayerBalanceQuery, decimal?>
    {
        private readonly IKYApiClient _client;
        private readonly ILogger<KYPlayerBalanceGetService> _logger;
        private readonly IPlayerRepository _playerRepository;
        private readonly IKYService _service;

        public KYPlayerBalanceGetService(
            ILogger<KYPlayerBalanceGetService> logger,
            IKYApiClient client,
            IKYService service,
            IPlayerRepository playerRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
        }

        public async ValueTask<decimal?> GetAsync(KYPlayerBalanceQuery command, CancellationToken cancellationToken = default)
        {
            var playerModel = await _playerRepository.GetAsync(command.PlayerCode).ConfigureAwait(false);
            if (playerModel == null)
            {
                throw new Exception($"The player does not exist.{command.PlayerCode}");
            }
            var brandMerchant = await _service.GetBrandMerchantAsync(playerModel.BrandCode).ConfigureAwait(false);
            var agentId = await _service.GetAgentIdAsync(playerModel.BrandCode, playerModel.DefaultWalletCurrencyCode).ConfigureAwait(false);

            return await _client.GetBalanceAsync(brandMerchant.ApiUrl, brandMerchant.Deskey, brandMerchant.Md5key, agentId, playerModel.PlayerCode).ConfigureAwait(false);
        }
    }
}
