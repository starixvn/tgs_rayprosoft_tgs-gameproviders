using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.Core;
using TGS.GameProviders.Core.KY.ApplicationServices;
using TGS.GameProviders.Repositories;
using TGS.ServiceUtility;

namespace TGS.GameProviders.KickPlayer.Core.ApplicationServices.KY
{
    public class KYKickPlayerCommandService : ICommandService<KYPlayerKickCommand>
    {
        private readonly ICommandService<KYKickPlayerCommand> _commandService;
        private readonly ILogger<KYKickPlayerCommandService> _logger;
        private readonly IPlayerRepository _playerRepository;
        private readonly IKYService _service;

        public KYKickPlayerCommandService(
            ICommandService<KYKickPlayerCommand> commandService,
            ILogger<KYKickPlayerCommandService> logger,
            IPlayerRepository playerRepository
            )
        {
            _commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger)); ;
            _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
        }

        public async ValueTask ExecuteAsync(KYPlayerKickCommand command, CancellationToken cancellationToken = default)
        {
            var player = await _playerRepository.GetPlayerAsync(command.PlayerCode).ConfigureAwait(false);
            if (player == null)
            {
                return;
            }
            await _commandService.ExecuteAsync(new()
            {
                BrandCode = player.BrandCode,
                PlayerCode = command.PlayerCode,
                CurrencyCode = player.DefaultWalletCurrencyCode
            });
        }
    }
}
