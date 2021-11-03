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

        public async ValueTask ExecuteAsync(VRKickPlayerCommand command, CancellationToken cancellationToken = default)
        {
            var brandMerchant = await _service.GetBrandMerchantAsync(command.BrandCode).ConfigureAwait(false);
            await _client.KickAsync(
                brandMerchant.ApiUrl,
                brandMerchant.Merchantid,
                brandMerchant.Version,
                brandMerchant.Deskey,
                new()
                {
                    PlayerCode = command.PlayerCode
                }).ConfigureAwait(false);
        }
    }
}
