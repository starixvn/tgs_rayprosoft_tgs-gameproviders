using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core.Repositories;

namespace TGS.GameProviders.Core.WGSports.ApplicationServices
{
    public class WGSportsKickPlayerCommandService : ICommandService<WGSportsKickPlayerCommand>
    {
        private readonly IWGSportsApiClient _client;
        private readonly ILogger<WGSportsKickPlayerCommandService> _logger;
        private readonly IPlayerRepository _playerRepository;
        private readonly IWGSportsService _service;

        public WGSportsKickPlayerCommandService(
            ILogger<WGSportsKickPlayerCommandService> logger,
            IWGSportsApiClient client,
            IWGSportsService service,
            IPlayerRepository playerRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
        }

        public async ValueTask ExecuteAsync(WGSportsKickPlayerCommand command, CancellationToken cancellationToken = default)
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
