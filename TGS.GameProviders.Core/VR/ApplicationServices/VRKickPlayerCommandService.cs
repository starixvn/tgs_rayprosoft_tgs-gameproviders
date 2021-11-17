using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core.Repositories;

namespace TGS.GameProviders.Core.VR.ApplicationServices
{
    public class VRKickPlayerCommandService : ICommandService<VRKickPlayerCommand>
    {
        private readonly IVRApiClient _client;
        private readonly ILogger<VRKickPlayerCommandService> _logger;
        private readonly IPlayerRepository _playerRepository;
        private readonly IVRService _service;

        public VRKickPlayerCommandService(
            IVRApiClient client,
            ILogger<VRKickPlayerCommandService> logger,
            IPlayerRepository playerRepository,
            IVRService service)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async ValueTask ExecuteAsync(VRKickPlayerCommand command, CancellationToken cancellationToken = default)
        {
            var playerModel = await _playerRepository.GetAsync(command.PlayerCode).ConfigureAwait(false);
            if (playerModel == null)
            {
                throw new Exception($"The player does not exist.{command.PlayerCode}");
            }

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
