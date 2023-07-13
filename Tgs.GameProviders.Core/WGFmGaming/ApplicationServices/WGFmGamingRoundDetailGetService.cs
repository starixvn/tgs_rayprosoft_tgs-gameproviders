using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.WGFmGaming.ApplicationServices;

public class WGFmGamingRoundDetailGetService : IGetService<WGFmGamingRoundDetailQuery, RoundDetailResult>
{
    private readonly IRoundRepository _roundService;

    public WGFmGamingRoundDetailGetService(IRoundRepository roundService)
    {
        _roundService = roundService ?? throw new ArgumentNullException(nameof(roundService));
    }

    public async ValueTask<RoundDetailResult> GetAsync(WGFmGamingRoundDetailQuery query, CancellationToken cancellationToken = default)
    {
        var roundInfo = await _roundService
            .GetAsync(
            roundId: query.RoundId,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        return roundInfo == null
            ? throw new Exception($"This game round number could not be found.{query.RoundId}")
            : new()
            {
                Data = new RoundDetailDataBase[]
                {
                    new RoundDetailDataJson { Value = roundInfo.AdditionalData }
                }
            };
    }
}
