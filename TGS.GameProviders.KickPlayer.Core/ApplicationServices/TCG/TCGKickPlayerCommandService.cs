using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.ApiClients.TCG;
using TGS.GameProviders.Core;
using TGS.GameProviders.Repositories;
using TGS.ServiceUtility;
using TGS.GameProviders.KickPlayer.Core.ApplicationServices.TCG;

namespace TGS.GameProviders.KickPlayer.Core.ApplicationServices.TCG
{
    public class TCGKickPlayerCommandService : ICommandService<TCGPlayerKickCommand>
    {
        private readonly ITCGApiClient _client;
        private readonly ILogger<TCGKickPlayerCommandService> _logger;
        private readonly IPlayerRepository _playerRepository;
        private readonly ITCGService _service;

        public TCGKickPlayerCommandService(
            ILogger<TCGKickPlayerCommandService> logger,
            ITCGApiClient client,
            ITCGService service,
            IPlayerRepository playerRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
        }

        public async ValueTask ExecuteAsync(TCGPlayerKickCommand command, CancellationToken cancellationToken = default)
        {
            var player = await _playerRepository.GetPlayerAsync(command.PlayerCode).ConfigureAwait(false);
            if (player == null)
            {
                return;
            }
            var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);
            var brandMerchant = await _service.GetMerchantAsync(player.DefaultWalletCurrencyCode).ConfigureAwait(false);

            await _client.KickOutLottoPlayerAsync(
                configuration.ApiUrl,
                brandMerchant, new()
                {
                    UserName = command.PlayerCode
                }).ConfigureAwait(false);
        }
    }
}
