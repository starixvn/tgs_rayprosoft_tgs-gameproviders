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
    private readonly IRoundRepository _roundService;
    private readonly IPragmaticPlayService _service;

    public PragmaticPlayRoundDetailGetService(
        IRoundRepository roundService,
        IGameActionService gameActionService,
        IPragmaticPlayService service,
        IPragmaticPlayApiClient client)
    {
        _roundService = roundService ?? throw new ArgumentNullException(nameof(roundService));
        _gameActionService = gameActionService ?? throw new ArgumentNullException(nameof(gameActionService));
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask<RoundDetailResult> GetAsync(
        PragmaticPlayRoundDetailQuery query,
        CancellationToken cancellationToken = default)
    {
        var roundInfo = await _roundService
            .GetAsync(
            roundId: query.RoundId,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var gameProviderModel = await _service
            .GetGameProviderAsync(
            gameProviderId: roundInfo.GameProviderId)
            .ConfigureAwait(false);

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
