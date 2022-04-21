using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.GameProviders.Core.PragmaticPlay.Models;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.PragmaticPlay.ApplicationServices
{
    public class PragmaticPlayKickPlayerCommandService : ICommandService<PragmaticPlayKickPlayerCommand>
    {
        private readonly IPragmaticPlayApiClient _client;
        private readonly ILogger<PragmaticPlayKickPlayerCommandService> _logger;
        private readonly IPragmaticPlayService _service;

        public PragmaticPlayKickPlayerCommandService(
            ILogger<PragmaticPlayKickPlayerCommandService> logger,
            IPragmaticPlayService service,
            IPragmaticPlayApiClient client)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async ValueTask ExecuteAsync(
            PragmaticPlayKickPlayerCommand command,
            CancellationToken cancellationToken = default)
        {
            var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);

            var request = new PragmaticPlayLogoutRequest
            {
                PlayerCode = command.PlayerCode,
                SecureLogin = configuration.SecureLogin
            };

            _ = await _client.LogoutAsync(configuration.ApiUrl, configuration.SecretKey, request).ConfigureAwait(false);
        }
    }
}
