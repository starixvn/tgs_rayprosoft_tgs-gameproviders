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

            var gameProviderCode = _service.GetGameProviderCode();
            var gameProviderId = await _service.GetGameProviderIdAsync().ConfigureAwait(false);

            var datas = response.Data.List.GameID.Select((gameId, index) => new GameRecordListModle()
            {
                GameProviderCode = gameProviderCode,
                GameProviderId = gameProviderId,
                PlayerCode = response.Data.List.Accounts[index].Split('_')[1],
                Status = RoundStatus.Closed,
                ClientStatus = _service.GetClientRound(Convert.ToDecimal(response.Data.List.Profit[index])),
                ExternalGameId = response.Data.List.KindID[index].ToString(),
                ExternalRoundId = response.Data.List.GameID[index],
                ExternalCreatedOn = new DateTimeOffset(DateTime.Parse(response.Data.List.GameStartTime[index]), TimeSpan.FromHours(+8)),
                ExternalClosedOn = new DateTimeOffset(DateTime.Parse(response.Data.List.GameEndTime[index]), TimeSpan.FromHours(+8)),
                BetAmount = Convert.ToDecimal(response.Data.List.AllBet[index]),
                ValidBetAmount = Convert.ToDecimal(response.Data.List.AllBet[index]),
                PlayerWinLoss = Convert.ToDecimal(response.Data.List.Profit[index]),
                Commission = Convert.ToDecimal(response.Data.List.Revenue[index]),
                Turnover = Convert.ToDecimal(response.Data.List.CellScore[index]),
                SettleAmount = Convert.ToDecimal(response.Data.List.AllBet[index]) + Convert.ToDecimal(response.Data.List.Profit[index]),
                TieBetAdjustment = null,
                AdditionalData = JsonSerializer.Serialize(new LYGameRecord()
                {
                    GameID = response.Data.List.GameID[index],
                    Account = response.Data.List.Accounts[index],
                    ServerID = response.Data.List.ServerID[index],
                    KindID = response.Data.List.KindID[index],
                    TableID = response.Data.List.TableID[index],
                    ChairID = response.Data.List.ChairID[index],
                    UserCount = response.Data.List.UserCount[index],
                    CellScore = Convert.ToDecimal(response.Data.List.CellScore[index]),
                    AllBet = Convert.ToDecimal(response.Data.List.AllBet[index]),
                    Profit = Convert.ToDecimal(response.Data.List.Profit[index]),
                    Revenue = Convert.ToDecimal(response.Data.List.Revenue[index]),
                    GameStartTime = new DateTimeOffset(DateTime.Parse(response.Data.List.GameStartTime[index]), TimeSpan.FromHours(+8)),
                    GameEndTime = new DateTimeOffset(DateTime.Parse(response.Data.List.GameEndTime[index]), TimeSpan.FromHours(+8)),
                    CardValue = response.Data.List.CardValue[index],
                    ChannelID = Convert.ToInt32(response.Data.List.ChannelID[index]),
                    LineCode = response.Data.List.LineCode[index]
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
