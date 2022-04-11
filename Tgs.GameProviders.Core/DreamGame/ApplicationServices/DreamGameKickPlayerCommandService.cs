using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.GameProviders.Core.Extensions;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.DreamGame.ApplicationServices
{
    public class DreamGameKickPlayerCommandService : ICommandService<DreamGameKickPlayerCommand>
    {
        private readonly IDreamGameApiClient _client;
        private readonly ILogger<DreamGameKickPlayerCommandService> _logger;
        private readonly IDreamGameService _service;

        public DreamGameKickPlayerCommandService(
            ILogger<DreamGameKickPlayerCommandService> logger,
            IDreamGameApiClient client,
            IDreamGameService service)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async ValueTask ExecuteAsync(DreamGameKickPlayerCommand command, CancellationToken cancellationToken = default)
        {
            var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);

            var onlineReportResults = await _client.OnlineReportAsync(configuration.ApiUrl, configuration.ApiKey, configuration.AgentName).ConfigureAwait(false);

            if (onlineReportResults != null)
            {
                var memberId = onlineReportResults.Where(x => x.Username.Contains(command.PlayerCode, StringComparison.OrdinalIgnoreCase)).Select(x => (int?)x.MemberId).FirstOrDefault();
                if (memberId.HasValue)
                {
                    _ = await _client.OfflineAsync(configuration.ApiUrl, configuration.ApiKey, configuration.AgentName, memberId.Value).ConfigureAwait(false);
                }
            }

            _logger.Log(
                LogLevel.Information,
                new EventId(
                    GameProviderUnique.DreamGame.GetGameProviderUniqueEventId(),
                    nameof(DreamGameKickPlayerCommandService)),
                new
                {
                    GameProviderCode = GameProviderType.DreamGame,
                    command.PlayerCode,
                    FinishOn = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(+8)).ToString("O")
                },
                null,
                (state, ex) => JsonSerializer.Serialize(state));
        }
    }
}
