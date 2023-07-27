using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.DreamGamingService.Client;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.DreamGame.ApplicationServices;

internal class DreamGameRoundDetailGetService : IGetService<DreamGameRoundDetailQuery, RoundDetailResult>
{
    private readonly IGameInterfaceDreamGamingClient _client;
    private readonly IRoundRepository _roundService;
    private readonly IDreamGameService _service;

    public DreamGameRoundDetailGetService(
        IGameInterfaceDreamGamingClient client,
        IRoundRepository roundService,
        IDreamGameService service
        )
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _roundService = roundService ?? throw new ArgumentNullException(nameof(roundService));
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    public async ValueTask<RoundDetailResult> GetAsync(DreamGameRoundDetailQuery query, CancellationToken cancellationToken = default)
    {
        var roundInfo = await _roundService
            .GetAsync(
            roundId: query.RoundId,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false)
            ?? throw new Exception($"This game round number could not be found.{query.RoundId}");

        var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);
        var gameProviderCultureCode = await _service.GetGameProviderLanguageCodeAsync(query.Language).ConfigureAwait(false);

        var reply = await _client
            .GameHistoryDetailAsync(
            videoGameApiUrl: configuration.VideoGameApiUrl,
            agentName: configuration.AgentName,
            language: gameProviderCultureCode,
            id: roundInfo.ExternalRoundId,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return new RoundDetailResult()
        {
            Data = new RoundDetailDataBase[]
                {
                    new RoundDetailDataUrl { Url = reply.GameResult }
                }
        };
    }
}
