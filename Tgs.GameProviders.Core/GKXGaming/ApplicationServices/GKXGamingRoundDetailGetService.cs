using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.GKXGaming.ApplicationServices;

internal class GKXGamingRoundDetailGetService : IGetService<GKXGamingRoundDetailQuery, RoundDetailResult>
{
    private readonly IRoundRepository _roundService;

    public GKXGamingRoundDetailGetService(
        IRoundRepository roundService)
    {
        _roundService = roundService ?? throw new ArgumentNullException(nameof(roundService));
    }

    public async ValueTask<RoundDetailResult> GetAsync(GKXGamingRoundDetailQuery query, CancellationToken cancellationToken = default)
    {
        var roundInfo = await _roundService
            .GetAsync(
            roundId: query.RoundId,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return new RoundDetailResult()
        {
            Data = new RoundDetailDataBase[]
            {
                new RoundDetailDataJson { Value = roundInfo.AdditionalData }
            }
        };
    }
}
