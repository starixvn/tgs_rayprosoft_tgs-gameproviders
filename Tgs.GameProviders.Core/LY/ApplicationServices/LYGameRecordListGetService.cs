using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.GameProviders.Core.LY.Models;
using Tgs.GameProviders.Domains;
using Tgs.GameRounds.Common;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.LY.ApplicationServices;

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
        var agentId = brandMerchant.AgentMappers.Where(x => _service.IgnoreCaseEquals(x.CurrencyCode, command.CurrencyCode) && x.Status).FirstOrDefault()?.AgentId;

        if (string.IsNullOrEmpty(agentId))
        {
            return new() { Success = true };
        }

        var dateTimeOffset = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8));

        (bool Success, LYGameRecordList GameRecordList) = await _client.GetGameRecordAsync(
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

        var gameProviderCode = GameProviderType.LY;
        var gameProviderId = await _service.GetGameProviderIdAsync().ConfigureAwait(false);

        var datas = GameRecordList.GameID.Select((gameId, index) => new GameRecordListModel()
        {
            GameProviderCode = gameProviderCode,
            GameProviderId = gameProviderId,
            PlayerCode = GameRecordList.Accounts[index].Split('_')[1],
            Status = GameRoundStatus.Closed,
            ClientStatus = _service.GetClientRound(Convert.ToDecimal(GameRecordList.Profit[index])),
            ExternalGameId = GameRecordList.KindID[index].ToString(),
            ExternalRoundId = GameRecordList.GameID[index],
            ExternalCreatedOn = new DateTimeOffset(DateTime.Parse(GameRecordList.GameStartTime[index]), TimeSpan.FromHours(+8)),
            ExternalClosedOn = new DateTimeOffset(DateTime.Parse(GameRecordList.GameEndTime[index]), TimeSpan.FromHours(+8)),
            BetAmount = Convert.ToDecimal(GameRecordList.AllBet[index]),
            ValidBetAmount = Convert.ToDecimal(GameRecordList.AllBet[index]),
            PlayerWinLoss = Convert.ToDecimal(GameRecordList.Profit[index]),
            Commission = Convert.ToDecimal(GameRecordList.Revenue[index]),
            Turnover = Convert.ToDecimal(GameRecordList.CellScore[index]),
            SettleAmount = Convert.ToDecimal(GameRecordList.AllBet[index]) + Convert.ToDecimal(GameRecordList.Profit[index]),
            TieBetAdjustment = null,
            AdditionalData = JsonSerializer.Serialize(new LYGameRecord()
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
