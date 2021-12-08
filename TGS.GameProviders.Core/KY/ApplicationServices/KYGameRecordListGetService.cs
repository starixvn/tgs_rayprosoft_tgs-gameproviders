using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core.Extensions;
using TGS.GameProviders.Core.KY.Models;
using TGS.GameProviders.Domains;

namespace TGS.GameProviders.Core.KY.ApplicationServices
{
    public class KYGameRecordListGetService : IGetService<KYGameRecordListQuery, GameRecordListResult>
    {
        private readonly IKYApiClient _client;
        private readonly ILogger<KYGameRecordListGetService> _logger;
        private readonly IKYService _service;

        public KYGameRecordListGetService(
            IKYApiClient client,
            ILogger<KYGameRecordListGetService> logger,
            IKYService service
            )
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async ValueTask<GameRecordListResult> GetAsync(KYGameRecordListQuery command, CancellationToken cancellationToken = default)
        {
            var brandMerchant = await _service.GetBrandMerchantAsync(command.BrandCode);
            var agentId = brandMerchant.AgentMappers.Where(x => x.CurrencyCode.IgnoreCaseEquals(command.CurrencyCode) && x.Status).FirstOrDefault()?.AgentId;

            if (string.IsNullOrEmpty(agentId))
            {
                return new() { Success = true };
            }

            (bool Success, KYGameRecordList GameRecordList) = await _client.GetGameRecordAsync(
                brandMerchant.ApiRecordUrl,
                brandMerchant.Deskey,
                brandMerchant.Md5key,
                agentId,
                command.StartTime.ToUnixTimeMilliseconds(),
                command.EndTime.ToUnixTimeMilliseconds()
                );

            if (!Success)
            {
                return new() { Success = false };
            }
            if (Success && GameRecordList == null)
            {
                return new() { Success = true };
            }

            var gameProviderCode = _service.GetGameProviderCode();
            var gameProviderId = await _service.GetGameProviderIdAsync().ConfigureAwait(false);

            var datas = GameRecordList.GameID.Select((gameId, index) => new GameRecordListModel()
            {
                GameProviderCode = gameProviderCode,
                GameProviderId = gameProviderId,
                PlayerCode = GameRecordList.Accounts[index].Split('_')[1],
                Status = RoundStatus.Closed,
                ClientStatus = _service.GetClientRound(Convert.ToDecimal(GameRecordList.Profit[index])),
                ExternalRoundId = GameRecordList.GameID[index],
                ExternalGameId = GameRecordList.KindID[index].ToString(),
                ExternalCreatedOn = new DateTimeOffset(DateTime.Parse(GameRecordList.GameStartTime[index]), TimeSpan.FromHours(+8)),
                ExternalClosedOn = new DateTimeOffset(DateTime.Parse(GameRecordList.GameEndTime[index]), TimeSpan.FromHours(+8)),
                BetAmount = Convert.ToDecimal(GameRecordList.AllBet[index]),
                ValidBetAmount = Convert.ToDecimal(GameRecordList.AllBet[index]),
                PlayerWinLoss = Convert.ToDecimal(GameRecordList.Profit[index]),
                Commission = Convert.ToDecimal(GameRecordList.Revenue[index]),
                Turnover = Convert.ToDecimal(GameRecordList.CellScore[index]),
                SettleAmount = Convert.ToDecimal(GameRecordList.AllBet[index]) + Convert.ToDecimal(GameRecordList.Profit[index]),
                TieBetAdjustment = null,
                AdditionalData = JsonSerializer.Serialize(new KYGameRecord()
                {
                    GameID = GameRecordList.GameID[index],
                    Account = GameRecordList.Accounts[index],
                    ServerID = GameRecordList.ServerID[index],
                    KindID = GameRecordList.KindID[index],
                    TableID = GameRecordList.TableID[index],
                    ChairID = GameRecordList.ChairID[index],
                    UserCount = GameRecordList.UserCount[index],
                    CellScore = Convert.ToDecimal(GameRecordList.CellScore[index]),
                    AllBet = Convert.ToDecimal(GameRecordList.AllBet[index]),
                    Profit = Convert.ToDecimal(GameRecordList.Profit[index]),
                    Revenue = Convert.ToDecimal(GameRecordList.Revenue[index]),
                    GameStartTime = new DateTimeOffset(DateTime.Parse(GameRecordList.GameStartTime[index]), TimeSpan.FromHours(+8)),
                    GameEndTime = new DateTimeOffset(DateTime.Parse(GameRecordList.GameEndTime[index]), TimeSpan.FromHours(+8)),
                    CardValue = GameRecordList.CardValue[index],
                    ChannelID = Convert.ToInt32(GameRecordList.ChannelID[index]),
                    LineCode = GameRecordList.LineCode[index]
                })
            }).ToArray();

            return new() { Success = true, Datas = datas };
        }
    }
}
