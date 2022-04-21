using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.GameProviders.Core.WMCasino.Models;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.WMCasino.ApplicationServices
{
    public class WMCasinoKickPlayerCommandService : ICommandService<WMCasinoKickPlayerCommand>
    {
        private readonly IWMCasinoAgentMapper _agentMapper;
        private readonly IWMCasinoApiClient _client;
        private readonly ILogger<WMCasinoKickPlayerCommandService> _logger;
        private readonly IWMCasinoService _service;
        private readonly IWMCasinoUtility _utility;

        public WMCasinoKickPlayerCommandService(
            ILogger<WMCasinoKickPlayerCommandService> logger,
            IWMCasinoApiClient client,
            IWMCasinoService service,
            IWMCasinoAgentMapper agentMapper,
            IWMCasinoUtility utility)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _agentMapper = agentMapper ?? throw new ArgumentNullException(nameof(agentMapper));
            _utility = utility ?? throw new ArgumentNullException(nameof(utility));
        }

        public async ValueTask ExecuteAsync(WMCasinoKickPlayerCommand command,
            CancellationToken cancellationToken = default)
        {
            var player = await _service.GetPlayerAsync(command.PlayerCode).ConfigureAwait(false);

            var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);

            var agent = _agentMapper.GetAgent(configuration.BrandMerchants, player.BrandCode,
                player.DefaultWalletCurrencyCode);

            var playerId = _utility.ToWMCasinoPlayerId(player.PlayerCode, agent.Prefix, agent.Suffix);

            _ = await _client.LogoutGameAsync(configuration.ApiUrl, new WMCasinoLogoutGameRequest
            {
                Signature = agent.Signature,
                VendorId = agent.AgentId,
                User = playerId
            }).ConfigureAwait(false);
        }
    }
}
