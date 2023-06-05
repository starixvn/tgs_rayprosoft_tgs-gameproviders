using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.YeeBet.ApplicationServices;

internal class YeeBetRoundDetailGetService : IGetService<YeeBetRoundDetailQuery, RoundDetailResult>
{
    private readonly IRoundRepository _roundService;

    public YeeBetRoundDetailGetService(
        IRoundRepository roundService)
    {
        _roundService = roundService ?? throw new ArgumentNullException(nameof(roundService));
    }

    public async ValueTask<RoundDetailResult> GetAsync(YeeBetRoundDetailQuery query, CancellationToken cancellationToken = default)
    {
        var roundInfo = await _roundService
            .GetAsync(
                query.RoundId)
            .ConfigureAwait(false)
            ?? throw new Exception($"This game round number could not be found.{query.RoundId}");

        return new RoundDetailResult()
        {
            Data = new RoundDetailDataBase[]
            {
                new RoundDetailDataJson {Value = roundInfo.AdditionalData}
            }
        };
    }
}
