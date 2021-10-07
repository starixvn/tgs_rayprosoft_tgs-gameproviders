using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.ApiClients.KY;
using TGS.GameProviders.Core;
using TGS.GameProviders.Repositories;
using TGS.ServiceUtility;
using TGS.GameProviders.KickPlayer.Core.ApplicationServices.KY;

namespace TGS.GameProviders.KickPlayer.Core.ApplicationServices.KY
{
    public class KYKickPlayerCommandService : ICommandService<KYPlayerKickCommand>
    {
        private readonly IKYApiClient _client;
        private readonly ILogger<KYKickPlayerCommandService> _logger;
        private readonly IPlayerRepository _playerRepository;
        private readonly IKYService _service;

        public KYKickPlayerCommandService(
            ILogger<KYKickPlayerCommandService> logger,
            IKYApiClient client,
            IKYService service,
            IPlayerRepository playerRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
        }

        public async ValueTask ExecuteAsync(KYPlayerKickCommand command, CancellationToken cancellationToken = default)
        {
            var player = await _playerRepository.GetPlayerAsync(command.PlayerCode).ConfigureAwait(false);
            var brandMerchant = await _service.GetBrandMerchantAsync(player.BrandCode).ConfigureAwait(false);
            var agentId = await _service.GetAgentIdAsync(player.BrandCode, player.DefaultWalletCurrencyCode).ConfigureAwait(false);
            await _client.KickAsync(brandMerchant, agentId, new()
            {
                PlayerCode = command.PlayerCode
            }).ConfigureAwait(false);
        }
    }
}
