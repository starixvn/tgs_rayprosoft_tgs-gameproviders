using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core.Repositories;

namespace TGS.GameProviders.Core.LY.ApplicationServices
{
    public class LYPlayerBalanceGetService : IGetService<LYPlayerBalanceQuery, decimal>
    {
        private readonly ILYApiClient _client;
        private readonly ILogger<LYPlayerBalanceGetService> _logger;
        private readonly IPlayerRepository _playerRepository;
        private readonly ILYService _service;

        public LYPlayerBalanceGetService(
            ILogger<LYPlayerBalanceGetService> logger,
            ILYApiClient client,
            ILYService service,
            IPlayerRepository playerRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
        }

        public async ValueTask<decimal> GetAsync(LYPlayerBalanceQuery command, CancellationToken cancellationToken = default)
        {
            var playerModel = await _playerRepository.GetAsync(command.PlayerCode).ConfigureAwait(false);
            if (playerModel == null)
            {
                throw new Exception($"The player does not exist.{command.PlayerCode}");
            }
            var brandMerchant = await _service.GetBrandMerchantAsync(playerModel.BrandCode).ConfigureAwait(false);
            var agentId = await _service.GetAgentIdAsync(playerModel.BrandCode, playerModel.DefaultWalletCurrencyCode).ConfigureAwait(false);

            var response = await _client.GetBalanceAsync(brandMerchant.ApiUrl, brandMerchant.Deskey, brandMerchant.Md5key, agentId, new()
            {
                PlayerCode = playerModel.PlayerCode
            }).ConfigureAwait(false);

            return response.Data.Balance;
        }
    }
}
