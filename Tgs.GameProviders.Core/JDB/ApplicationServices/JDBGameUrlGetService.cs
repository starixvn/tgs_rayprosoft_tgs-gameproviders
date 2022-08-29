using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameProviders.Core.JDB.Models;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.JDB.ApplicationServices;

internal class JDBGameUrlGetService : IGetService<JDBGameUrlQuery, GameUrlResult>
{
    private readonly IJDBAgentMapper _agentMapper;
    private readonly IJDBApiClient _client;
    private readonly IGameRepository _gameRepository;
    private readonly IPlayerRepository _playerRepository;
    private readonly IJDBService _service;

    public JDBGameUrlGetService(
        IPlayerRepository playerRepository,
        IGameRepository gameRepository,
        IJDBService service,
        IJDBAgentMapper agentMapper,
        IJDBApiClient client)
    {
        _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
        _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _agentMapper = agentMapper ?? throw new ArgumentNullException(nameof(agentMapper));
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask<GameUrlResult> GetAsync(
        JDBGameUrlQuery query,
        CancellationToken cancellationToken = default)
    {
        var playerInfo = await _playerRepository.GetAsync(query.PlayerCode).ConfigureAwait(false);
        var gameInfo = await _gameRepository.GetAsync(query.GameId!.Value).ConfigureAwait(false);

        var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);
        var agent = _agentMapper.GetAgent(configuration.Agents, playerInfo!.DefaultWalletCurrencyCode);

        var gameProviderLanguageCode = await _service
            .GetGameProviderLanguageAsync(gameInfo!.GameProviderId, query.Language)
            .ConfigureAwait(false);

        var items = gameInfo.GameExternalId.Split('_');

        var request = new JDBGetTokenRequest
        {
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            Parent = agent.Id,
            UserId = playerInfo.PlayerCode,
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
