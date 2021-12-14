using System;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core.VR.Models;
using TGS.GameProviders.Domains;

namespace TGS.GameProviders.Core.Avia.ApplicationServices
{
    public class AviaGameRecordListGetService : IGetService<AviaGameRecordListQuery, GameRecordListResult>
    {
        private readonly IAviaApiClient _client;
        private readonly ILogger<AviaGameRecordListGetService> _logger;
        private readonly IAviaService _service;

        public AviaGameRecordListGetService(
            IAviaApiClient client,
            ILogger<AviaGameRecordListGetService> logger,
            IAviaService service
            )
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async ValueTask<GameRecordListResult> GetAsync(AviaGameRecordListQuery command, CancellationToken cancellationToken = default)
        {
            var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);

            (bool Success, Models.AviaGameRecordData[]? GameRecords) = await _client.GetGameRecordAsync(
               configuration.ApiUrl,
               configuration.Authorization,
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
                ExternalCreatedOn = DateTimeOffset.Parse(data.CreateAt).ToOffset(TimeSpan.FromHours(+8)),
                ExternalClosedOn = _service.GetRoundStatus(data.Status) == RoundStatus.Closed ? DateTimeOffset.Parse(data.RewardAt).ToOffset(TimeSpan.FromHours(+8)) : null,
                BetAmount = data.BetAmount,
                ValidBetAmount = data.BetMoney,
                PlayerWinLoss = data.Money,
                Commission = 0,
                Turnover = data.BetMoney,
                SettleAmount = data.BetMoney + data.Money,
                TieBetAdjustment = null,
                AdditionalData = JsonSerializer.Serialize(data),
            })
               .ToArray();

            return new() { Success = true, Datas = datas };
        }
    }
}
