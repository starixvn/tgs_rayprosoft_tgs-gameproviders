using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;

namespace TGS.GameProviders.Core.Avia.ApplicationServices
{
    public class AviaKickPlayerCommandService : ICommandService<AviaKickPlayerCommand>
    {
        private readonly IAviaApiClient _client;
        private readonly ILogger<AviaKickPlayerCommandService> _logger;
        private readonly IPlayerService _playerService;
        private readonly IAviaService _service;

        public AviaKickPlayerCommandService(
            ILogger<AviaKickPlayerCommandService> logger,
            IAviaApiClient client,
            IAviaService service,
            IPlayerService playerService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
        }

        public async ValueTask ExecuteAsync(AviaKickPlayerCommand command, CancellationToken cancellationToken = default)
        {
            var playerInfo = await _playerService.GetCacheAsync(command.PlayerCode).ConfigureAwait(false);
            if (playerInfo == null)
            {
                throw new Exception($"The player does not exist.{command.PlayerCode}");
            }
            var brandMerchant = await _service.GetBrandMerchantAsync(playerInfo.BrandCode);

            if (brandMerchant == null)
            {
                throw new ArgumentNullException(nameof(brandMerchant));
            }

            await _client.KickAsync(
                brandMerchant.ApiUrl,
                brandMerchant.Authorization,
                command.PlayerCode).ConfigureAwait(false);
        }
    }
}
