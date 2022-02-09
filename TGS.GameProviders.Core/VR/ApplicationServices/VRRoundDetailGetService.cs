using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core.Repositories;
using TGS.GameProviders.Domains.RoundDetail;

namespace TGS.GameProviders.Core.VR.ApplicationServices
{
    internal class VRRoundDetailGetService : IGetService<VRRoundDetailQuery, RoundDetailResult>
    {
        private readonly IRoundService _roundService;

        public VRRoundDetailGetService(IRoundService roundService)
        {
            _roundService = roundService ?? throw new ArgumentNullException(nameof(roundService));
        }

        public async ValueTask<RoundDetailResult> GetAsync(VRRoundDetailQuery query, CancellationToken cancellationToken = default)
        {
            var roundInfo = await _roundService.GetAsync(query.RoundId).ConfigureAwait(false);
            if (roundInfo == null)
            {
                throw new Exception($"This game round number could not be found.{query.RoundId}");
            }

            return new()
            {
                Data = new RoundDetailDataBase[]
                {
                    new RoundDetailDataJson { Value = roundInfo.AdditionalData }
                }
            };
        }
    }
}
