using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameProviders.Core.Skywind.Models;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.Skywind.ApplicationServices;

internal class SkywindRoundDetailGetService : IGetService<SkywindRoundDetailQuery, RoundDetailResult>
{
    private readonly ISkywindApiClient _client;
    private readonly IRoundRepository _roundService;
    private readonly ISkywindService _service;
    private readonly ISkywindAccessToken _skywindAccessToken;

    public SkywindRoundDetailGetService(
        ISkywindApiClient client,
        IRoundRepository roundService,
        ISkywindService service,
        ISkywindAccessToken skywindAccessToken
        )
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _roundService = roundService ?? throw new ArgumentNullException(nameof(roundService));
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _skywindAccessToken = skywindAccessToken ?? throw new ArgumentNullException(nameof(skywindAccessToken));
    }

    public async ValueTask<RoundDetailResult> GetAsync(SkywindRoundDetailQuery query, CancellationToken cancellationToken = default)
    {
        var roundInfo = await _roundService.GetAsync(query.RoundId).ConfigureAwait(false);
        if (roundInfo == null)
        {
            throw new Exception($"This game round number could not be found.{query.RoundId}");
        }

        var roundId = roundInfo.ExternalRoundId;
        if (roundId.IndexOf("_") != -1)
        {
            roundId = roundId.Split("_")[0];
        }

        var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);

        var getaccessToken = await _skywindAccessToken.GetAccessTokenAsync().ConfigureAwait(false);
        var response = await _client
            .GetGamHistoryRoundDetailsAsync(
            configuration.ApiUrl,
            getaccessToken,
            new SkywindGamHistoryRoundDetailsRequest
            {
                RoundId = roundId
            }).ConfigureAwait(false);

        return new()
        {
            Data = new RoundDetailDataBase[]
            {
                new RoundDetailDataUrl { Url = response.ImageUrl }
            }
        };
    }
}
