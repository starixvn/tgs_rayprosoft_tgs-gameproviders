using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core.BigGaming.Models;
using TGS.GameProviders.Core.BigGaming.Models.Commons;
using TGS.GameProviders.Core.Extensions;
using TGS.GameProviders.Domains;

namespace TGS.GameProviders.Core.BigGaming.ApplicationServices
{
    public class BigGamingFishingGameRecordListGetService : IGetService<BigGamingFishingGameRecordListQuery, GameRecordListResult>
    {
        private readonly IBigGamingApiClient _client;
        private readonly ILogger<BigGamingFishingGameRecordListGetService> _logger;
        private readonly IBigGamingService _service;

        public BigGamingFishingGameRecordListGetService(
            IBigGamingApiClient client,
            ILogger<BigGamingFishingGameRecordListGetService> logger,
            IBigGamingService service
            )
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async ValueTask<GameRecordListResult> GetAsync(BigGamingFishingGameRecordListQuery command, CancellationToken cancellationToken = default)
        {
            var brandMerchant = await _service.GetBrandMerchantAsync(command.CurrencyCode).ConfigureAwait(false);

            var response = await _client.GetGameRecordAsync(
                brandMerchant.ApiUrl,
                brandMerchant.SecretCode,
                brandMerchant.SecretKey,
                new()
                {
                    Random = GuidExtensions.NewSequentialGuid().ToString(),
                    Sn = brandMerchant.SecretCode,
                    StartTime = command.StartTime.ToOffset(TimeSpan.FromHours(-4)).ToString("yyyy-MM-dd HH:mm:ss"),
                    EndTime = command.EndTime.ToOffset(TimeSpan.FromHours(-4)).ToString("yyyy-MM-dd HH:mm:ss"),
                    PageIndex = command.Page.ToString(),
                    PageSize = command.Rows
                }).ConfigureAwait(false);

            if (response.Error != null)
            {
                return new()
                {
                    Success = false
                };
            }

            var gameRecords = JsonSerializer.Deserialize<BigGamingResultGameRecordResponse<BigGamingGameRecordFishingResponse>>(JsonSerializer.Serialize(response.Result));

            if (gameRecords?.Items == null || gameRecords?.Items.Length == 0)
            {
                return new()
                {
                    Success = true
                };
            }

            var gameProviderCode = _service.GetGameProviderCode();
            var gameProviderId = await _service.GetGameProviderIdAsync().ConfigureAwait(false);

            var datas = gameRecords.Items.Select(data => new GameRecordListModel()
            {
                GameProviderCode = gameProviderCode,
                GameProviderId = gameProviderId,
                PlayerCode = data.LoginId,
                Status = RoundStatus.Closed,
                ClientStatus = _service.GetClientRound(data.Payout),
                ExternalRoundId = data.ExternalBetId.ToString(),
                ExternalGameId = data.GameType.ToString(),
                ExternalCreatedOn = new DateTimeOffset(DateTime.Parse(data.OrderTime), TimeSpan.FromHours(-4)),
                ExternalClosedOn = new DateTimeOffset(DateTime.Parse(data.OrderTime), TimeSpan.FromHours(-4)),
                BetAmount = data.BetAmount,
                ValidBetAmount = data.ValidAmount,
                PlayerWinLoss = data.Payout,
                Commission = data.Extend,
                Turnover = data.ValidAmount,
                SettleAmount = data.ValidAmount + data.Payout,
                TieBetAdjustment = null,
                AdditionalData = JsonSerializer.Serialize(data)
            }).ToArray();

            return new() { Success = true, Datas = datas };
        }
    }
}
