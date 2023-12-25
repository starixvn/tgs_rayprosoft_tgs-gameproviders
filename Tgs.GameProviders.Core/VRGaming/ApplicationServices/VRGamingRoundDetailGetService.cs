using System.Threading;
using System.Threading.Tasks;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.VRGaming.ApplicationServices;

internal class VRGamingRoundDetailGetService(IRoundRepository roundRepository) : IGetService<VRGamingRoundDetailQuery, RoundDetailResult>
{
    public async ValueTask<RoundDetailResult> GetAsync(VRGamingRoundDetailQuery query, CancellationToken cancellationToken = default)
    {
        var roundInfo = await roundRepository.GetAsync(
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
