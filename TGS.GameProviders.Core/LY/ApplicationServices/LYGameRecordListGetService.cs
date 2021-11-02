using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using TGS.GameProviders.ApiClients;
using TGS.GameProviders.ApiClients.LY.Models;
using TGS.GameProviders.ApiClients.LY.Models.Commons;
using TGS.GameProviders.Domains.Extensions;
using TGS.GameProviders.Repositories.Domains;

namespace TGS.GameProviders.Core.LY.ApplicationServices
{
    public class LYGameRecordListGetService : IGetService<LYGameRecordListQuery, GameRecordListResult>
    {
        private readonly ILYApiClient _client;
        private readonly ILogger<LYGameRecordListGetService> _logger;
        private readonly ILYService _service;

        public LYGameRecordListGetService(
            ILYApiClient client,
            ILogger<LYGameRecordListGetService> logger,
            ILYService service
            )
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async ValueTask<GameRecordListResult> GetAsync(LYGameRecordListQuery command, CancellationToken cancellationToken = default)
        {
            var gameProviderId = await _service.GetGameProviderIdAsync().ConfigureAwait(false);
            var brandMerchant = await _service.GetBrandMerchantAsync(command.BrandCode);
            var agentId = brandMerchant.AgentMappers.Where(x => x.CurrencyCode.IgnoreCaseEquals(command.CurrencyCode) && x.Status).FirstOrDefault()?.AgentId;

            if (string.IsNullOrEmpty(agentId))
            {
                return new() { Success = true };
            }

            var dateTimeOffset = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8));

            var data = new LYGameRecordRequest()
            {
                StartTime = command.StartTime.ToUnixTimeMilliseconds(),
                EndTime = command.EndTime.ToUnixTimeMilliseconds()
            };

            var response = await _client.GetGameRecordAsync(brandMerchant.ApiRecordUrl, brandMerchant.Deskey, brandMerchant.Md5key, agentId, dateTimeOffset, data);

            if (response == null || response?.Data.Code != 0)
            {
                if (response?.Data.Code == 16)
                {
                    return new() { Success = true };
                }
                else
                {
                    return new() { Success = false };
                }
            }

            if (response?.Data == null || response?.Data.List.GameID.Length == 0)
            {
                return new() { Success = true };
            }

            var datas = response.Data.List.GameID.Select((x, i) => new GameRecordListModle()
            {
                ExternalRoundId = response.Data.List.GameID[i],
                PlayerCode = response.Data.List.Accounts[i].Split('_')[1],
                ExternalGameId = response.Data.List.KindID[i].ToString(),
                Status = RoundStatus.Closed,
                ExternalCreatedOn = new DateTimeOffset(DateTime.Parse(response.Data.List.GameStartTime[i]), TimeSpan.FromHours(+8)),
                ExternalClosedOn = new DateTimeOffset(DateTime.Parse(response.Data.List.GameEndTime[i]), TimeSpan.FromHours(+8)),
                BetAmount = Convert.ToDecimal(response.Data.List.AllBet[i]),
                ValidBetAmount = Convert.ToDecimal(response.Data.List.AllBet[i]),
                PlayerWinLoss = Convert.ToDecimal(response.Data.List.Profit[i]),
                Commission = Convert.ToDecimal(response.Data.List.Revenue[i]),
                Turnover = Convert.ToDecimal(response.Data.List.CellScore[i]),
                SettleAmount = Convert.ToDecimal(response.Data.List.AllBet[i]) + Convert.ToDecimal(response.Data.List.Profit[i]),
                TieBetAdjustment = null,
                ClientStatus = _service.GetClientRound(Convert.ToDecimal(response.Data.List.Profit[i])),
                AdditionalData = JsonSerializer.Serialize(new LYGameRecord()
                {
                    GameID = response.Data.List.GameID[i],
                    Account = response.Data.List.Accounts[i],
                    ServerID = response.Data.List.ServerID[i],
                    KindID = response.Data.List.KindID[i],
                    TableID = response.Data.List.TableID[i],
                    ChairID = response.Data.List.ChairID[i],
                    UserCount = response.Data.List.UserCount[i],
                    CellScore = Convert.ToDecimal(response.Data.List.CellScore[i]),
                    AllBet = Convert.ToDecimal(response.Data.List.AllBet[i]),
                    Profit = Convert.ToDecimal(response.Data.List.Profit[i]),
                    Revenue = Convert.ToDecimal(response.Data.List.Revenue[i]),
                    GameStartTime = new DateTimeOffset(DateTime.Parse(response.Data.List.GameStartTime[i]), TimeSpan.FromHours(+8)),
                    GameEndTime = new DateTimeOffset(DateTime.Parse(response.Data.List.GameEndTime[i]), TimeSpan.FromHours(+8)),
                    CardValue = response.Data.List.CardValue[i],
                    ChannelID = Convert.ToInt32(response.Data.List.ChannelID[i]),
                    LineCode = response.Data.List.LineCode[i]
                })
            }).ToArray();

            return new()
            {
                Success = true,
                Datas = datas
            };
        }
    }
}
