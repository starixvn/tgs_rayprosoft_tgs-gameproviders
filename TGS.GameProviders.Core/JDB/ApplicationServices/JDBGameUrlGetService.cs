using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core.JDB.Models;

namespace TGS.GameProviders.Core.JDB.ApplicationServices;

internal class JDBGameUrlGetService : IGetService<JDBGameUrlQuery, GameUrlResult>
{
    private readonly IJDBAgentMapper _agentMapper;
    private readonly IJDBApiClient _client;
    private readonly IGameService _gameService;
    private readonly ILogger<JDBGameUrlGetService> _logger;
    private readonly IPlayerService _playerService;
    private readonly IJDBService _service;

    public JDBGameUrlGetService(
        ILogger<JDBGameUrlGetService> logger,
        IPlayerService playerService,
        IGameService gameService,
        IJDBService service,
        IJDBAgentMapper agentMapper,
        IJDBApiClient client)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
        _gameService = gameService ?? throw new ArgumentNullException(nameof(gameService));
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _agentMapper = agentMapper ?? throw new ArgumentNullException(nameof(agentMapper));
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask<GameUrlResult> GetAsync(
        JDBGameUrlQuery query,
        CancellationToken cancellationToken = default)
    {
        var playerModel = await _playerService.GetCacheAsync(query.PlayerCode).ConfigureAwait(false);
        var gameModel = await _gameService.GetCacheAsync(query.GameId!.Value).ConfigureAwait(false);

        var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);
        var agent = _agentMapper.GetAgent(configuration.Agents, playerModel!.DefaultWalletCurrencyCode);

        var gameProviderLanguageCode = await _service
            .GetGameProviderLanguageAsync(gameModel!.GameProviderId, query.Language)
            .ConfigureAwait(false);

        var items = gameModel.GameExternalId.Split('_');

        var request = new JDBGetTokenRequest
        {
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            Parent = agent.Id,
            UserId = playerModel.PlayerCode,
            Language = gameProviderLanguageCode,
            GameType = items[0],
            MachineType = items[1],
            MoreGame = configuration.MoreGame ? 1 : 0
        };

        var response = await _client
            .GetTokenAsync(
                configuration.ApiUrl,
                configuration.ClientDomain,
                configuration.AESIv,
                configuration.AESKey,
                request)
            .ConfigureAwait(false);

        if (response.Status != "0000")
        {
            return null;
        }

        return new GameUrlResult
        {
            Url = new Uri(response.Path)
        };
    }
}
