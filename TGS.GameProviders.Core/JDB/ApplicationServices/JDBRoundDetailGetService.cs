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
    private readonly IGameRepository _gameRepository;
    private readonly ILogger<JDBRoundDetailGetService> _logger;
    private readonly IPlayerService _playerService;
    private readonly IRoundRepository _roundRepository;
    private readonly IJDBService _service;

    public JDBRoundDetailGetService(
        ILogger<JDBRoundDetailGetService> logger,
        IRoundRepository roundRepository,
        IPlayerService playerService,
        IGameProviderRepository gameProviderRepository,
        IGameRepository gameRepository,
        IJDBService service,
        IJDBAgentMapper agentMapper,
        IJDBApiClient client)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _roundRepository = roundRepository ?? throw new ArgumentNullException(nameof(roundRepository));
        _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
        _gameProviderRepository = gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
        _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _agentMapper = agentMapper ?? throw new ArgumentNullException(nameof(agentMapper));
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask<RoundDetailResult> GetAsync(
        JDBRoundDetailQuery query,
        CancellationToken cancellationToken = default)
    {
        var roundModel = await _roundRepository.GetAsync(query.RoundId).ConfigureAwait(false);
        if (roundModel == null)
        {
            throw new Exception($"This game round number could not be found.{query.RoundId}");
        }

        var playerModel = await _playerService.GetCacheAsync(roundModel.PlayerCode).ConfigureAwait(false);
        if (playerModel == null)
        {
            throw new Exception($"The player does not exist.{roundModel.PlayerCode}");
        }

        var gameProviderModel = await _gameProviderRepository.GetAsync(roundModel.GameProviderId).ConfigureAwait(false);
        if (gameProviderModel == null)
        {
            throw new Exception($"This game manufacturer does not exist.{roundModel.GameProviderId}");
        }

        var gameModel = await _gameRepository.GetAsync(roundModel.GameId).ConfigureAwait(false);
        if (gameModel == null)
        {
            throw new Exception($"This game Id could not be found.{roundModel.GameId}");
        }

        var gameProviderLanguageCode = await _service
            .GetGameProviderLanguageAsync(gameProviderModel.GameProviderId, query.Language)
            .ConfigureAwait(false);

        var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);
        var agent = _agentMapper.GetAgent(configuration.Agents, playerModel.DefaultWalletCurrencyCode);

        var items = gameModel.GameExternalId.Split("_");
        if (items.Length != 2 || !int.TryParse(items[0], out var gtype))
        {
            throw new ArgumentException($"Invalid external game id '{gameModel.GameExternalId}'");
        }

        if (!long.TryParse(roundModel.ExternalRoundId, out var externalRoundId))
        {
            throw new ArgumentException("Invalid external round id");
        }

        var request = new JDBBetHistoryRequest
        {
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            Parent = agent.Id,
            UserId = playerModel.PlayerCode,
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
            GameProviderName = gameProviderModel.Name,
            GameName = gameModel.Name,
            ExternalRoundId = roundModel.ExternalRoundId,
            BetAmount = roundModel.BetAmount,
            Turnover = roundModel.Turnover,
            Winloss = roundModel.WinLoss,
            GameResult = roundModel.ClientStatus.ToString(),
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
