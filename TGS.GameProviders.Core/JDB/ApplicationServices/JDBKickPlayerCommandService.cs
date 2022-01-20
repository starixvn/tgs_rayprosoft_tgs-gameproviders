using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core.JDB.Models;
using TGS.GameProviders.Core.Repositories;

namespace TGS.GameProviders.Core.JDB.ApplicationServices;

internal class JDBKickPlayerCommandService : ICommandService<JDBKickPlayerCommand>
{
    private readonly ILogger<JDBKickPlayerCommandService> _logger;
    private readonly IPlayerRepository _playerRepository;
    private readonly IJDBService _service;
    private readonly IJDBAgentMapper _agentMapper;
    private readonly IJDBApiClient _client;

    public JDBKickPlayerCommandService(
        ILogger<JDBKickPlayerCommandService> logger,
        IPlayerRepository playerRepository,
        IJDBService service,
        IJDBAgentMapper agentMapper,
        IJDBApiClient client)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _agentMapper = agentMapper ?? throw new ArgumentNullException(nameof(agentMapper));
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask ExecuteAsync(
        JDBKickPlayerCommand command,
        CancellationToken cancellationToken = default)
    {
        var playerModel = await _playerRepository.GetAsync(command.PlayerCode).ConfigureAwait(false);
        if (playerModel == null)
        {
            throw new Exception($"The player does not exist.{command.PlayerCode}");
        }

        var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);
        var agent = _agentMapper.GetAgent(configuration.Agents, playerModel.DefaultWalletCurrencyCode);

        var request = new JDBLogoutRequest
        {
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            Parent = agent.Id,
            UserId = playerModel.PlayerCode,
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