using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core.Repositories;
using TGS.GameProviders.Domains.RoundDetail;

namespace TGS.GameProviders.Core.Jili.ApplicationServices;

internal class JiliRoundDetailGetService : IGetService<JiliRoundDetailQuery, RoundDetailResult>
{
    private readonly ILogger<JiliRoundDetailGetService> _logger;
    private readonly IRoundRepository _roundRepository;
    private readonly IGameProviderRepository _gameProviderRepository;
    private readonly IGameRepository _gameRepository;
    private readonly IPlayerRepository _playerRepository;
    private readonly IJiliService _service;
    private readonly IJiliMerchantMapper _merchantMapper;
    private readonly IJiliApiClient _client;

    public JiliRoundDetailGetService(
        ILogger<JiliRoundDetailGetService> logger,
        IRoundRepository roundRepository,
        IGameProviderRepository gameProviderRepository,
        IGameRepository gameRepository,
        IPlayerRepository playerRepository,
        IJiliService service,
        IJiliMerchantMapper merchantMapper,
        IJiliApiClient client)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _roundRepository = roundRepository ?? throw new ArgumentNullException(nameof(roundRepository));
        _gameProviderRepository = gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
        _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
        _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _merchantMapper = merchantMapper ?? throw new ArgumentNullException(nameof(merchantMapper));
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask<RoundDetailResult> GetAsync(
        JiliRoundDetailQuery query,
        CancellationToken cancellationToken = default)
    {
        var roundModel = await _roundRepository.GetAsync(query.RoundId).ConfigureAwait(false);
        if (roundModel == null)
        {
            throw new Exception($"This game round number could not be found.{query.RoundId}");
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

        var playerModel = await _playerRepository.GetAsync(roundModel.PlayerCode).ConfigureAwait(false);
        if (playerModel == null)
        {
            throw new Exception($"This player could not be found.{roundModel.PlayerCode}");
        }

        var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);
        var merchant = _merchantMapper.GetMerchant(configuration.Merchants, playerModel.BrandCode);

        var gameProviderCultureCode = await _service
            .GetGameProviderLanguageCodeAsync(roundModel.GameProviderId, query.Language)
            .ConfigureAwait(false);

        var response = await _client
            .GetGameDetailUrlAsync(configuration.ApiUrl,
                merchant.AgentId,
                merchant.AgentKey,
                gameProviderCultureCode,
                roundModel.ExternalRoundId)
            .ConfigureAwait(false);

        var url = response.ErrorCode == 0 ? response.Data.Url : string.Empty;

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
                    Url = url
                }
            }
        };
    }
}