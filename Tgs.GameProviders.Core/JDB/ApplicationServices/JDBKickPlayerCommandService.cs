using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.GameProviders.Core.JDB.Models;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.JDB.ApplicationServices;

internal class JDBKickPlayerCommandService : ICommandService<JDBKickPlayerCommand>
{
    private readonly IJDBAgentMapper _agentMapper;
    private readonly IJDBApiClient _client;
    private readonly ILogger<JDBKickPlayerCommandService> _logger;
    private readonly IPlayerService _playerService;
    private readonly IJDBService _service;

    public JDBKickPlayerCommandService(
        ILogger<JDBKickPlayerCommandService> logger,
        IPlayerService playerService,
        IJDBService service,
        IJDBAgentMapper agentMapper,
        IJDBApiClient client)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _agentMapper = agentMapper ?? throw new ArgumentNullException(nameof(agentMapper));
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask ExecuteAsync(
        JDBKickPlayerCommand command,
        CancellationToken cancellationToken = default)
    {
        var playerInfo = await _playerService.GetCacheAsync(command.PlayerCode).ConfigureAwait(false);
        if (playerInfo == null)
        {
            throw new Exception($"The player does not exist.{command.PlayerCode}");
        }

        var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);
        var agent = _agentMapper.GetAgent(configuration.Agents, playerInfo.DefaultWalletCurrencyCode);

        var request = new JDBLogoutRequest
        {
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            Parent = agent.Id,
            UserId = playerInfo.PlayerCode,
        };

        _ = await _client.LogoutAsync(
                configuration.ApiUrl,
                configuration.ClientDomain,
                configuration.AESIv,
                configuration.AESKey,
                request)
            .ConfigureAwait(false);
    }
}
