using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.Core.SexyGaming.ApplicationServices;
using TGS.GameProviders.Repositories;
using TGS.ServiceUtility;

namespace TGS.GameProviders.KickPlayer.Core.ApplicationServices.SexyGaming
{
    public class SexyGamingKickPlayerCommandService : ICommandService<SexyGamingPlayerKickCommand>
    {
        private readonly ICommandService<SexyGamingKickPlayerCommand> _commandService;
        private readonly ILogger<SexyGamingKickPlayerCommandService> _logger;
        private readonly IPlayerRepository _playerRepository;

        public SexyGamingKickPlayerCommandService(
            ICommandService<SexyGamingKickPlayerCommand> commandService,
            ILogger<SexyGamingKickPlayerCommandService> logger,
            IPlayerRepository playerRepository
            )
        {
            _commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
        }

        public async ValueTask ExecuteAsync(SexyGamingPlayerKickCommand command, CancellationToken cancellationToken = default)
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
