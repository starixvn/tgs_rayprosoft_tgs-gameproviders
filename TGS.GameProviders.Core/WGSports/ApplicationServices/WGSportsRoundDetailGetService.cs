using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core.Repositories;
using TGS.GameProviders.Domains.RoundDetail;

namespace TGS.GameProviders.Core.WGSports.ApplicationServices
{
    internal class WGSportsRoundDetailGetService : IGetService<WGSportsRoundDetailQuery, RoundDetailResult>
    {
        private readonly IRoundRepository _roundRepository;

        public WGSportsRoundDetailGetService(
            IRoundRepository roundRepository
            )
        {
            _roundRepository = roundRepository ?? throw new ArgumentNullException(nameof(roundRepository));
        }

        public async ValueTask<RoundDetailResult> GetAsync(WGSportsRoundDetailQuery query, CancellationToken cancellationToken = default)
        {
            var round = await _roundRepository.GetRoundAsync(query.RoundId).ConfigureAwait(false);
            if (round == null)
            {
                throw new Exception($"This game round number could not be found.{query.RoundId}");
            }

            return new()
            {
                BetAmount = round.BetAmount ?? 0,
                Turnover = round.Turnover ?? 0,
                Winloss = round.WinLoss ?? 0,
                GameResult = round.ClientStatus.ToString(),
                Data = new RoundDetailDataBase[]
                {
                    new RoundDetailDataJson {Value = round.AdditionalData}
                }
            };
        }
    }
}
