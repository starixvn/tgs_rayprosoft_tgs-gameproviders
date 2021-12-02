using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core.Saba.Models;

namespace TGS.GameProviders.Core.Saba.ApplicationServices
{
    internal class SabaKickPlayerCommandService : ICommandService<SabaKickPlayerCommand>
    {
        private readonly ISabaApiClient _client;
        private readonly ILogger<SabaKickPlayerCommandService> _logger;
        private readonly ISabaService _service;
        private readonly ISabaUtility _utility;

        public SabaKickPlayerCommandService(
            ILogger<SabaKickPlayerCommandService> logger,
            ISabaService service,
            ISabaUtility utility,
            ISabaApiClient client)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _utility = utility ?? throw new ArgumentNullException(nameof(utility));
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async ValueTask ExecuteAsync(SabaKickPlayerCommand command, CancellationToken cancellationToken = default)
        {
            var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);
            var isTestEnv = _service.IsTestEnvironment(configuration.TestEnvironments, command.EnvironmentName);

            var playerId = _utility.ToSabaPlayerId(command.PlayerCode, isTestEnv);

            var request = new SabaKickUserRequest
            {
                VendorId = configuration.VendorId,
                PlayerId = playerId
            };

            _ = await _client.KickUserAsync(configuration.ApiUrl, request).ConfigureAwait(false);
        }
    }
}
