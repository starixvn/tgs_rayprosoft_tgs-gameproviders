using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.LEGaming.ApplicationServices;

internal class LEGamingRoundDetailGetService : IGetService<LEGamingRoundDetailQuery, RoundDetailResult>
{
    private readonly IRoundRepository _roundService;

    public LEGamingRoundDetailGetService(
        IRoundRepository roundService)
    {
        _roundService = roundService ?? throw new ArgumentNullException(nameof(roundService));
    }

    public async ValueTask<RoundDetailResult> GetAsync(LEGamingRoundDetailQuery query, CancellationToken cancellationToken = default)
    {
        var roundInfo = await _roundService
            .GetAsync(
            roundId: query.RoundId,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return roundInfo == null
            ? throw new Exception($"This game round number could not be found.{query.RoundId}")
            : new RoundDetailResult()
            {
                Data = new RoundDetailDataBase[]
                {
                    new RoundDetailDataJson { Value = roundInfo.AdditionalData }
                }
            };
    }
}
