using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core.PragmaticPlay.Models;
using TGS.GameProviders.Core.Repositories;
using TGS.GameProviders.Domains.RoundDetail;

namespace TGS.GameProviders.Core.PragmaticPlay.ApplicationServices;

internal class PragmaticPlayRoundDetailGetService : IGetService<PragmaticPlayRoundDetailQuery, RoundDetailResult>
{
    private readonly IPragmaticPlayApiClient _client;
    private readonly IGameActionService _gameActionService;
    private readonly ILogger<PragmaticPlayRoundDetailGetService> _logger;
    private readonly IRoundRepository _roundRepository;
    private readonly IPragmaticPlayService _service;
    private readonly IPragmaticPlayUtility _utility;

    public PragmaticPlayRoundDetailGetService(
        ILogger<PragmaticPlayRoundDetailGetService> logger,
        IRoundRepository roundRepository,
        IGameActionService gameActionService,
        IPragmaticPlayService service,
        IPragmaticPlayUtility utility,
        IPragmaticPlayApiClient client)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _roundRepository = roundRepository ?? throw new ArgumentNullException(nameof(roundRepository));
        _gameActionService = gameActionService ?? throw new ArgumentNullException(nameof(gameActionService));
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _utility = utility ?? throw new ArgumentNullException(nameof(utility));
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask<RoundDetailResult> GetAsync(
        PragmaticPlayRoundDetailQuery query,
        CancellationToken cancellationToken = default)
    {
        var roundModel = await _roundRepository.GetAsync(query.RoundId).ConfigureAwait(false);
        if (roundModel == null)
        {
            throw new Exception($"This game round number could not be found.{query.RoundId}");
        }

        var gameProviderModel = await _service.GetGameProviderAsync(roundModel.GameProviderId).ConfigureAwait(false);
        if (gameProviderModel == null)
        {
            throw new Exception($"This game manufacturer does not exist.{roundModel.GameProviderId}");
        }

        var gameModel = await _service.GetGameAsync(roundModel.GameId).ConfigureAwait(false);
        if (gameModel == null)
        {
            throw new Exception($"This game Id could not be found.{roundModel.GameId}");
        }

        var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);

        var gameProviderCultureCode = await _service
            .GetGameProviderLanguageAsync(gameProviderModel.GameProviderId, query.Language)
            .ConfigureAwait(false);

        var gameActions = await _gameActionService
            .QueryAsync(query.RoundId)
            .ToArrayAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var request = new PragmaticPlayOpenHistoryExtendedRequest
        {
            SecureLogin = configuration.SecureLogin,
            PlayerCode = roundModel.PlayerCode,
            RoundId = gameActions.OrderBy(ga => ga.RecordedOn).First().ExternalGameActionGroupId,
            Language = gameProviderCultureCode
        };

        var response = await _client
            .GetRoundHistoryAsync(configuration.ApiUrl, configuration.SecretKey, request)
            .ConfigureAwait(false);

        var url = response.Error == "0" ? response.Url : string.Empty;

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
