using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;

namespace TGS.GameProviders.Core.LY.ApplicationServices
{
    public class LYPlayerBalanceGetService : IGetService<LYPlayerBalanceQuery, decimal?>
    {
        private readonly ILYApiClient _client;
        private readonly ILogger<LYPlayerBalanceGetService> _logger;
        private readonly IPlayerService _playerService;
        private readonly ILYService _service;

        public LYPlayerBalanceGetService(
            ILogger<LYPlayerBalanceGetService> logger,
            ILYApiClient client,
            ILYService service,
            IPlayerService playerService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
        }

        public async ValueTask<decimal?> GetAsync(LYPlayerBalanceQuery command, CancellationToken cancellationToken = default)
        {
            var playerInfo = await _playerService.GetCacheAsync(command.PlayerCode).ConfigureAwait(false);
            if (playerInfo == null)
            {
                throw new Exception($"The player does not exist.{command.PlayerCode}");
            }
            var brandMerchant = await _service.GetBrandMerchantAsync(playerInfo.BrandCode).ConfigureAwait(false);
            var agentId = await _service.GetAgentIdAsync(playerInfo.BrandCode, playerInfo.DefaultWalletCurrencyCode).ConfigureAwait(false);

            return await _client.GetBalanceAsync(
                brandMerchant.ApiUrl,
                brandMerchant.Deskey,
                brandMerchant.Md5key,
                agentId,
                playerInfo.PlayerCode).ConfigureAwait(false);
        }
    }
}
