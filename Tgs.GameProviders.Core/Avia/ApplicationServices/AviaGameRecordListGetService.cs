using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameRounds.Common;
using Tgs.ServiceUtility;

#nullable enable

namespace Tgs.GameProviders.Core.Avia.ApplicationServices;

public class AviaGameRecordListGetService : IGetService<AviaGameRecordListQuery, GameRecordListResult>
{
    private readonly IAviaApiClient _client;
    private readonly IAviaService _service;

    public AviaGameRecordListGetService(
        IAviaApiClient client,
        IAviaService service
        )
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    public async ValueTask<GameRecordListResult> GetAsync(AviaGameRecordListQuery command, CancellationToken cancellationToken = default)
    {
        var brandMerchant = await _service.GetBrandMerchantAsync(command.BrandCode);

        if (brandMerchant == null)
        {
            throw new ArgumentNullException(nameof(brandMerchant));
        }

        (bool Success, Models.AviaGameRecordData[]? GameRecords) = await _client.GetGameRecordAsync(
           brandMerchant.ApiUrl,
           brandMerchant.Authorization,
           command.StartTime.ToOffset(TimeSpan.FromHours(8)),
           command.EndTime.ToOffset(TimeSpan.FromHours(8)),
           command.Page,
           command.Rows);

        if (!Success)
        {
            return new() { Success = false };
        }

        if (Success && GameRecords == null)
        {
            return new() { Success = true };
        }

        var gameProviderCode = _service.GetGameProviderCode();
        var gameProviderId = await _service.GetGameProviderIdAsync().ConfigureAwait(false);

        var datas = GameRecords.Select(data => new GameRecordListModel()
        {
            GameProviderCode = gameProviderCode,
            GameProviderId = gameProviderId,
            PlayerCode = data.UserName,
            Status = _service.GetRoundStatus(data.Status),
            ClientStatus = _service.GetClientRound(data.Status),
            ExternalRoundId = data.OrderID,
            ExternalGameId = "4",
            ExternalCreatedOn = new DateTimeOffset(DateTime.Parse(data.CreateAt), TimeSpan.FromHours(+8)),
            ExternalClosedOn = _service.GetRoundStatus(data.Status) == GameRoundStatus.Closed ? new DateTimeOffset(DateTime.Parse(data.RewardAt), TimeSpan.FromHours(+8)) : null,
            BetAmount = data.BetAmount,
            ValidBetAmount = data.BetMoney,
            PlayerWinLoss = data.Money,
            Commission = 0,
            Turnover = data.BetMoney,
            SettleAmount = _service.GetClientRound(data.Status) is GameRoundClientStatus.Win ? (data.BetAmount + data.Money) : 0,
            TieBetAdjustment = null,
            AdditionalData = JsonSerializer.Serialize(data),
        }).ToArray();

        return new() { Success = true, Datas = datas };
    }
}
