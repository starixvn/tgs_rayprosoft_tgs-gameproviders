using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using TGS.GameProviders.ApiClients;
using TGS.GameProviders.Repositories;

namespace TGS.GameProviders.Core.KY.ApplicationServices
{
    public class KYPlayerBalanceGetService : IGetService<KYPlayerBalanceQuery, PlayerBalanceResult>
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

        public async ValueTask<PlayerBalanceResult> GetAsync(KYPlayerBalanceQuery command, CancellationToken cancellationToken = default)
        {
            var brandMerchant = await _service.GetBrandMerchantAsync(command.BrandCode).ConfigureAwait(false);
            var agentId = await _service.GetAgentIdAsync(command.BrandCode, command.CurrencyCode).ConfigureAwait(false);

            var response = await _client.GetBalanceAsync(brandMerchant.ApiUrl, brandMerchant.Deskey, brandMerchant.Md5key, agentId, new()
            {
                PlayerCode = command.PlayerCode
            }).ConfigureAwait(false);

            return new()
            {
                Balance = response.Data.Balance
            };
        }
    }
}
