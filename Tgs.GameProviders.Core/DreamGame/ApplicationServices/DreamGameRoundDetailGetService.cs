using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.DreamGame.ApplicationServices;

internal class DreamGameRoundDetailGetService : IGetService<DreamGameRoundDetailQuery, RoundDetailResult>
{
    private readonly IDreamGameApiClient _client;
    private readonly IRoundRepository _roundService;
    private readonly IDreamGameService _service;

    public DreamGameRoundDetailGetService(
        IDreamGameApiClient client,
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
        var roundInfo = await _roundService.GetAsync(query.RoundId).ConfigureAwait(false);
        if (roundInfo == null)
        {
            throw new Exception($"This game round number could not be found.{query.RoundId}");
        }

        var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);
        var gameProviderCultureCode = await _service.GetGameProviderLanguageCodeAsync(query.Language).ConfigureAwait(false);

        var agentFix = await _client.GetAgentFixAsync(configuration.VideoGameApiUrl, configuration.AgentName).ConfigureAwait(false);

        return new()
        {
            Data = new RoundDetailDataBase[]
            {
                new RoundDetailDataUrl { Url = _client.GetGameResultAsync(configuration.VideoGameApiUrl, configuration.AgentName,gameProviderCultureCode,agentFix,roundInfo.ExternalRoundId) }
}
        };
    }
}
