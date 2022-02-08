using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core.JDB.Models;
using TGS.GameProviders.Core.Repositories;
using TGS.GameProviders.Domains.RoundDetail;

namespace TGS.GameProviders.Core.JDB.ApplicationServices;

internal class JDBRoundDetailGetService : IGetService<JDBRoundDetailQuery, RoundDetailResult>
{
    private readonly IJDBAgentMapper _agentMapper;
    private readonly IJDBApiClient _client;
    private readonly IGameProviderRepository _gameProviderRepository;
    private readonly IGameService _gameService;
    private readonly ILogger<JDBRoundDetailGetService> _logger;
    private readonly IPlayerService _playerService;
    private readonly IRoundService _roundService;
    private readonly IJDBService _service;

    public JDBRoundDetailGetService(
        ILogger<JDBRoundDetailGetService> logger,
        IRoundService roundService,
        IPlayerService playerService,
        IGameProviderRepository gameProviderRepository,
        IGameService gameService,
        IJDBService service,
        IJDBAgentMapper agentMapper,
        IJDBApiClient client)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _roundService = roundService ?? throw new ArgumentNullException(nameof(roundService));
        _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
        _gameProviderRepository = gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
        _gameService = gameService ?? throw new ArgumentNullException(nameof(gameService));
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _agentMapper = agentMapper ?? throw new ArgumentNullException(nameof(agentMapper));
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask<RoundDetailResult> GetAsync(
        JDBRoundDetailQuery query,
        CancellationToken cancellationToken = default)
    {
        var roundModel = await _roundService.GetAsync(query.RoundId).ConfigureAwait(false);
        if (roundModel == null)
        {
            throw new Exception($"This game round number could not be found.{query.RoundId}");
        }

        var playerInfo = await _playerService.GetCacheAsync(roundModel.PlayerCode).ConfigureAwait(false);
        if (playerInfo == null)
        {
            throw new Exception($"The player does not exist.{roundModel.PlayerCode}");
        }

        var gameProviderModel = await _gameProviderRepository.GetAsync(roundModel.GameProviderId).ConfigureAwait(false);
        if (gameProviderModel == null)
        {
            throw new Exception($"This game manufacturer does not exist.{roundModel.GameProviderId}");
        }

        var gameInfo = await _gameService.GetCacheAsync(roundModel.GameId).ConfigureAwait(false);
        if (gameInfo == null)
        {
            throw new Exception($"This game Id could not be found.{roundModel.GameId}");
        }

        var gameProviderLanguageCode = await _service
            .GetGameProviderLanguageAsync(gameProviderModel.GameProviderId, query.Language)
            .ConfigureAwait(false);

        var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);
        var agent = _agentMapper.GetAgent(configuration.Agents, playerInfo.DefaultWalletCurrencyCode);

        var items = gameInfo.GameExternalId.Split("_");
        if (items.Length != 2 || !int.TryParse(items[0], out var gtype))
        {
            throw new ArgumentException($"Invalid external game id '{gameInfo.GameExternalId}'");
        }

        if (!long.TryParse(roundModel.ExternalRoundId, out var externalRoundId))
        {
            throw new ArgumentException("Invalid external round id");
        }

        var request = new JDBBetHistoryRequest
        {
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            Parent = agent.Id,
            UserId = playerInfo.PlayerCode,
            Language = gameProviderLanguageCode,
            GameType = gtype,
            SequenceNo = externalRoundId
        };

        var response = await _client
            .GetBetHistoryUrlAsync(
                configuration.ApiUrl,
                configuration.ClientDomain,
                configuration.AESIv,
                configuration.AESKey,
                request)
            .ConfigureAwait(false);

        var url = default(string);
        if (response.Status == "0000")
        {
            url = response.Data.FirstOrDefault()?.Path;
        }

        return new RoundDetailResult
        {
            Data = new RoundDetailDataBase[]
            {
                new RoundDetailDataUrl
                {
                    Url = url ?? string.Empty
                }
            }
        };
    }
}
