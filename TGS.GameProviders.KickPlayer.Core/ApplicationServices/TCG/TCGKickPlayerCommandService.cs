using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.Core.TCG.ApplicationServices;
using TGS.GameProviders.Repositories;
using TGS.ServiceUtility;

namespace TGS.GameProviders.KickPlayer.Core.ApplicationServices.TCG
{
    public class TCGKickPlayerCommandService : ICommandService<TCGPlayerKickCommand>
    {
        private readonly ICommandService<TCGKickPlayerCommand> _commandService;
        private readonly ILogger<TCGKickPlayerCommandService> _logger;
        private readonly IPlayerRepository _playerRepository;

        public TCGKickPlayerCommandService(
            ICommandService<TCGKickPlayerCommand> commandService,
            ILogger<TCGKickPlayerCommandService> logger,
            IPlayerRepository playerRepository
            )
        {
            _commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
        }

        public async ValueTask ExecuteAsync(TCGPlayerKickCommand command, CancellationToken cancellationToken = default)
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
