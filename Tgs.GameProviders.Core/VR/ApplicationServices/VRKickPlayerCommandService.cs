using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.VR.ApplicationServices
{
    public class VRKickPlayerCommandService : ICommandService<VRKickPlayerCommand>
    {
        private readonly IVRApiClient _client;
        private readonly ILogger<VRKickPlayerCommandService> _logger;
        private readonly IPlayerRepository _playerService;
        private readonly IVRService _service;

        public VRKickPlayerCommandService(
            IVRApiClient client,
            ILogger<VRKickPlayerCommandService> logger,
            IPlayerRepository playerService,
            IVRService service)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async ValueTask ExecuteAsync(VRKickPlayerCommand command, CancellationToken cancellationToken = default)
        {
            var playerModel = await _playerService.GetAsync(command.PlayerCode).ConfigureAwait(false);
            var brandMerchant = await _service.GetBrandMerchantAsync(playerModel.BrandCode).ConfigureAwait(false);

            await _client.KickAsync(
                brandMerchant.ApiUrl,
                brandMerchant.Merchantid,
                brandMerchant.Version,
                brandMerchant.Deskey,
                new()
                {
                    PlayerCode = playerModel.PlayerCode
                }).ConfigureAwait(false);
        }
    }
}
