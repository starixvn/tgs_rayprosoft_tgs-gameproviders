using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.ApiClients;
using TGS.GameProviders.Core;
using TGS.GameProviders.Repositories;
using TGS.ServiceUtility;

namespace TGS.GameProviders.KickPlayer.Core.ApplicationServices.VR
{
    public class VRKickPlayerCommandService : ICommandService<VRPlayerKickCommand>
    {
        private readonly IVRApiClient _client;
        private readonly ILogger<VRKickPlayerCommandService> _logger;
        private readonly IPlayerRepository _playerRepository;
        private readonly IVRService _service;

        public VRKickPlayerCommandService(
            ILogger<VRKickPlayerCommandService> logger,
            IVRApiClient client,
            IVRService service,
            IPlayerRepository playerRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
        }

        public async ValueTask ExecuteAsync(VRPlayerKickCommand command, CancellationToken cancellationToken = default)
        {
            var player = await _playerRepository.GetPlayerAsync(command.PlayerCode).ConfigureAwait(false);
            if (player == null)
            {
                return;
            }
            var brandMerchant = await _service.GetBrandMerchantAsync(player.BrandCode).ConfigureAwait(false);
            await _client.KickAsync(brandMerchant, new()
            {
                PlayerCode = command.PlayerCode
            }).ConfigureAwait(false);
        }
    }
}
