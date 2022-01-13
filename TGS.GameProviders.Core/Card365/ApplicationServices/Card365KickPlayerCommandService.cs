using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.Core.Repositories;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core.Card365.Model;

namespace TGS.GameProviders.Core.Card365.ApplicationServices;

internal class Card365KickPlayerCommandService : ICommandService<Card365KickPlayerCommand>
{
    private readonly ILogger<Card365KickPlayerCommandService> _logger;
    private readonly IPlayerRepository _playerRepository;
    private readonly ICard365Service _service;
    private readonly ICard365AgentMapper _agentMapper;
    private readonly ICard365ApiClient _client;

    public Card365KickPlayerCommandService(
        ILogger<Card365KickPlayerCommandService> logger,
        IPlayerRepository playerRepository,
        ICard365Service service,
        ICard365AgentMapper agentMapper,
        ICard365ApiClient client)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _agentMapper = agentMapper ?? throw new ArgumentNullException(nameof(agentMapper));
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask ExecuteAsync(
        Card365KickPlayerCommand command,
        CancellationToken cancellationToken = default)
    {
        var playerModel = await _playerRepository.GetAsync(command.PlayerCode).ConfigureAwait(false);
        if (playerModel == null)
        {
            throw new Exception($"The player does not exist.{command.PlayerCode}");
        }

        var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);

        var agent = _agentMapper.GetAgent(configuration.BrandAgents, playerModel.BrandCode);

        var request = new Card365LogoutRequest
        {
            Cert = configuration.Cert,
            AgentId = agent.AgentId,
            PlayerCode = playerModel.PlayerCode
        };

        _ = await _client.LogoutAsync(
                configuration.ApiUrl,
                configuration.Website,
                request)
            .ConfigureAwait(false);
    }
}