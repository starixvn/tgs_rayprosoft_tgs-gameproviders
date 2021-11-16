using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core.LY.Models.Commons;
using TGS.GameProviders.Core.Repositories;
using TGS.GameProviders.Domains.RoundDetail;

namespace TGS.GameProviders.Core.LY.ApplicationServices
{
    internal class LYRoundDetailGetService : IGetService<LYRoundDetailQuery, RoundDetailResult>
    {
        private readonly IRoundRepository _roundRepository;

        public LYRoundDetailGetService(
            IRoundRepository roundRepository)
        {
            _roundRepository = roundRepository ?? throw new ArgumentNullException(nameof(roundRepository));
        }

        public async ValueTask<RoundDetailResult> GetAsync(LYRoundDetailQuery query, CancellationToken cancellationToken = default)
        {
            var round = await _roundRepository.GetRoundAsync(query.RoundId).ConfigureAwait(false);
            if (round == null)
            {
                throw new Exception($"This game round number could not be found.{query.RoundId}");
            }

            var gameRecord = JsonSerializer.Deserialize<LYGameRecord>(round.AdditionalData, new JsonSerializerOptions
            {
                // 設定在做序列化/反序列化時，尊重名稱大小寫
                PropertyNameCaseInsensitive = true
            });

            if (gameRecord == null)
            {
                return new()
                {
                    BetAmount = round.BetAmount ?? 0,
                    Turnover = round.Turnover ?? 0,
                    Winloss = round.WinLoss ?? 0,
                    GameResult = round.ClientStatus.ToString(),
                    Data = new RoundDetailDataBase[]
                {
                    new RoundDetailDataJson {Value = null}
                }
                };
            }

            return new()
            {
                BetAmount = round.BetAmount ?? 0,
                Turnover = round.Turnover ?? 0,
                Winloss = round.WinLoss ?? 0,
                GameResult = round.ClientStatus.ToString(),
                Data = new RoundDetailDataBase[]
                {
                    new RoundDetailDataJson {Value = gameRecord.CardValue}
                }
            };
        }
    }
}
