using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.IDNLive.ApplicationServices;

internal class IDNLiveRoundDetailGetService : IGetService<IDNLiveRoundDetailQuery, RoundDetailResult>
{
    private readonly IRoundRepository _roundService;

    public IDNLiveRoundDetailGetService(IRoundRepository roundService)
    {
        _roundService = roundService ?? throw new ArgumentNullException(nameof(roundService));
    }

    public async ValueTask<RoundDetailResult> GetAsync(IDNLiveRoundDetailQuery query, CancellationToken cancellationToken = default)
    {
        var roundInfo = await _roundService.GetAsync(query.RoundId).ConfigureAwait(false);
        return roundInfo == null
            ? throw new Exception($"This game round number could not be found.{query.RoundId}")
            : (new RoundDetailResult()
            {
                Data = new RoundDetailDataBase[]
                {
                    new RoundDetailDataJson {Value = roundInfo.AdditionalData}
                }
            });
    }
}
