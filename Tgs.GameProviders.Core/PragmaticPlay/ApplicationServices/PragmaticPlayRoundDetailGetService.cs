using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.GameProviders.Core.PragmaticPlay.Models;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.PragmaticPlay.ApplicationServices;

internal class PragmaticPlayRoundDetailGetService : IGetService<PragmaticPlayRoundDetailQuery, RoundDetailResult>
{
    private readonly IPragmaticPlayApiClient _client;
    private readonly IGameActionService _gameActionService;
    private readonly ILogger<PragmaticPlayRoundDetailGetService> _logger;
    private readonly IRoundService _roundService;
    private readonly IPragmaticPlayService _service;
    private readonly IPragmaticPlayUtility _utility;

    public PragmaticPlayRoundDetailGetService(
        ILogger<PragmaticPlayRoundDetailGetService> logger,
        IRoundService roundService,
        IGameActionService gameActionService,
        IPragmaticPlayService service,
        IPragmaticPlayUtility utility,
        IPragmaticPlayApiClient client)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _roundService = roundService ?? throw new ArgumentNullException(nameof(roundService));
        _gameActionService = gameActionService ?? throw new ArgumentNullException(nameof(gameActionService));
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _utility = utility ?? throw new ArgumentNullException(nameof(utility));
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask<RoundDetailResult> GetAsync(
        PragmaticPlayRoundDetailQuery query,
        CancellationToken cancellationToken = default)
    {
        var roundInfo = await _roundService.GetAsync(query.RoundId).ConfigureAwait(false);
        if (roundInfo == null)
        {
            throw new Exception($"This game round number could not be found.{query.RoundId}");
        }

        var gameProviderModel = await _service.GetGameProviderAsync(roundInfo.GameProviderId).ConfigureAwait(false);
        if (gameProviderModel == null)
        {
            throw new Exception($"This game manufacturer does not exist.{roundInfo.GameProviderId}");
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
            PlayerCode = roundInfo.PlayerCode,
            RoundId = gameActions.OrderBy(ga => ga.RecordedOn).First().ExternalGameActionGroupId,
            Language = gameProviderCultureCode
        };

        var response = await _client
            .GetRoundHistoryAsync(configuration.ApiUrl, configuration.SecretKey, request)
            .ConfigureAwait(false);

        var url = response.Error == "0" ? response.Url : string.Empty;

        return new RoundDetailResult
        {
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
