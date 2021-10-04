using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.ApiClients.BigGaming;
using TGS.GameProviders.Core;
using TGS.GameProviders.Repositories;
using TGS.ServiceUtility;

namespace TGS.GameProviders.KickPlayerReceiver.ApplicationServices.BigGaming
{
    public class BigGamingKickPlayerCommandService : ICommandService<BigGamingPlayerKickCommand>
    {
        private readonly IBigGamingApiClient _client;
        private readonly ILogger<BigGamingKickPlayerCommandService> _logger;
        private readonly IPlayerRepository _playerRepository;
        private readonly IBigGamingService _service;

        public BigGamingKickPlayerCommandService(
            ILogger<BigGamingKickPlayerCommandService> logger,
            IBigGamingApiClient client,
            IBigGamingService service,
            IPlayerRepository playerRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
        }

        public async ValueTask ExecuteAsync(BigGamingPlayerKickCommand command, CancellationToken cancellationToken = default)
        {
            var player = await _playerRepository.GetPlayerAsync(command.PlayerCode).ConfigureAwait(false);
            var brandMerchant = await _service.GetBrandMerchantAsync(player.DefaultWalletCurrencyCode).ConfigureAwait(false);

            await _client.KickPlayerAsync(brandMerchant, new()
            {
                Random = Guid.NewGuid().ToString(),
                Sn = brandMerchant.SecretCode,
                LoginId = "4ucmqplxb029"
            }).ConfigureAwait(false);
        }
    }
}
