using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core.Repositories;

namespace TGS.GameProviders.Core.Avia.ApplicationServices
{
    public class AviaKickPlayerCommandService : ICommandService<AviaKickPlayerCommand>
    {
        private readonly IAviaApiClient _client;
        private readonly ILogger<AviaKickPlayerCommandService> _logger;
        private readonly IPlayerRepository _playerRepository;
        private readonly IAviaService _service;

        public AviaKickPlayerCommandService(
            ILogger<AviaKickPlayerCommandService> logger,
            IAviaApiClient client,
            IAviaService service,
            IPlayerRepository playerRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
        }

        public async ValueTask ExecuteAsync(AviaKickPlayerCommand command, CancellationToken cancellationToken = default)
        {
            var playerModel = await _playerRepository.GetAsync(command.PlayerCode).ConfigureAwait(false);
            if (playerModel == null)
            {
                throw new Exception($"The player does not exist.{command.PlayerCode}");
            }
            var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);
            await _client.KickAsync(
                configuration.ApiUrl,
                configuration.Authorization,
                command.PlayerCode).ConfigureAwait(false);
        }
    }
}
