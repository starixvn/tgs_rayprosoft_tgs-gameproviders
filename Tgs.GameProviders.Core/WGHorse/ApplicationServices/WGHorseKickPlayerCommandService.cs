using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.WGHorse.ApplicationServices
{
    public class WGHorseKickPlayerCommandService : ICommandService<WGHorseKickPlayerCommand>
    {
        private readonly IWGHorseApiClient _client;
        private readonly ILogger<WGHorseKickPlayerCommandService> _logger;
        private readonly IPlayerService _playerService;
        private readonly IWGHorseService _service;

        public WGHorseKickPlayerCommandService(
            ILogger<WGHorseKickPlayerCommandService> logger,
            IWGHorseApiClient client,
            IWGHorseService service,
            IPlayerService playerService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
        }

        public async ValueTask ExecuteAsync(WGHorseKickPlayerCommand command, CancellationToken cancellationToken = default)
        {
            var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);

            await _client.LogoutAsync(
               configuration.ApiUrl,
               configuration.PartnerCode,
               configuration.PartnerKey,
               configuration.IV,
               command.PlayerCode).ConfigureAwait(false);
        }
    }
}
