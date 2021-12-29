using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core.AllBet.Models;
using TGS.GameProviders.Core.Repositories;

namespace TGS.GameProviders.Core.AllBet.ApplicationServices;

internal class AllBetKickPlayerCommandService : ICommandService<AllBetKickPlayerCommand>
{
    private readonly IAllBetAgentMapper _agentMapper;
    private readonly IAllBetApiClient _client;
    private readonly ILogger<AllBetKickPlayerCommandService> _logger;
    private readonly IPlayerRepository _playerRepository;
    private readonly IAllBetService _service;

    public AllBetKickPlayerCommandService(
        ILogger<AllBetKickPlayerCommandService> logger,
        IPlayerRepository playerRepository,
        IAllBetService service,
        IAllBetAgentMapper agentMapper,
        IAllBetApiClient client)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _agentMapper = agentMapper ?? throw new ArgumentNullException(nameof(agentMapper));
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask ExecuteAsync(
        AllBetKickPlayerCommand command,
        CancellationToken cancellationToken = default)
    {
        var playerModel = await _playerRepository.GetAsync(command.PlayerCode).ConfigureAwait(false);
        if (playerModel == null)
        {
            throw new Exception($"The player does not exist.{command.PlayerCode}");
        }

        var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);

        var agent = _agentMapper.GetAgent(configuration.BrandMerchants,
            playerModel.BrandCode,
            playerModel.DefaultWalletCurrencyCode);

        var allBetPlayerId = _service.ToAllBetPlayerId(playerModel.PlayerCode, agent.Suffix);

        var request = new AllBetLogoutRequest
        {
            PlayerId = allBetPlayerId
        };

        _ = await _client.LogoutAsync(
                configuration.ApiUrl,
                agent.PropertyId,
                agent.AllBetApiKey,
                request)
            .ConfigureAwait(false);
    }
}
