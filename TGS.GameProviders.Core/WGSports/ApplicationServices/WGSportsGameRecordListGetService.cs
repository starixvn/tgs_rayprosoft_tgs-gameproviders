using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core.Extensions;
using TGS.GameProviders.Core.WGSports.Models;
using TGS.GameProviders.Domains;

namespace TGS.GameProviders.Core.WGSports.ApplicationServices
{
    public class WGSportsGameRecordListGetService : IGetService<WGSportsGameRecordListQuery, GameRecordListResult>
    {
        private readonly IWGSportsApiClient _client;
        private readonly ILogger<WGSportsGameRecordListGetService> _logger;
        private readonly IWGSportsService _service;

        public WGSportsGameRecordListGetService(
            IWGSportsApiClient client,
            ILogger<WGSportsGameRecordListGetService> logger,
            IWGSportsService service
            )
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async ValueTask<GameRecordListResult> GetAsync(WGSportsGameRecordListQuery command, CancellationToken cancellationToken = default)
        {
            var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);

            var data = new WGSportsGameRecordRequest
            {
                GameRecordTime = (command.IsSettlement) ? GameRecordTime.SettleDate : GameRecordTime.BetDate,
                StartTime = command.StartTime.ToOffset(TimeSpan.FromHours(-4)).ToString("yyyy-MM-dd HH:mm:ss"),
                EndTime = command.EndTime.ToOffset(TimeSpan.FromHours(-4)).ToString("yyyy-MM-dd HH:mm:ss"),
            };

            var response = await _client.GetGameRecordAsync(
                configuration.ApiUrl,
                configuration.PartnerCode,
                configuration.PartnerKey,
                configuration.IV,
                data);

            if (response == null)
            {
                return new() { Success = false };
            }

            if (response.ReturnCode.IgnoreCaseNotEquals("1"))
            {
                return new() { Success = false };
            }

            if (response.BetInfo == null || response.BetInfo.Length == 0)
            {
                return new() { Success = true };
            }

            var gameProviderCode = _service.GetGameProviderCode();
            var gameProviderId = await _service.GetGameProviderIdAsync().ConfigureAwait(false);

            var datas = response.BetInfo
                .Where(data => data.Reference.IgnoreCaseNotContains("不接受"))
                .Select(data => new GameRecordListModle()
                {
                    GameProviderCode = gameProviderCode,
                    GameProviderId = gameProviderId,
                    PlayerCode = data.Username,
                    Status = data.InitData.Win.IgnoreCaseEquals("0") ? RoundStatus.Open : RoundStatus.Closed,
                    ClientStatus = data.InitData.Win.IgnoreCaseEquals("0") ? ClientRoundStatus.Open : _service.GetClientRound(decimal.Parse(data.Result)),
                    ExternalRoundId = data.Reference,
                    ExternalGameId = "1",
                    ExternalCreatedOn = new DateTimeOffset(DateTime.Parse(data.RecordDate), TimeSpan.FromHours(-4)),
                    ExternalClosedOn = data.InitData.Win.IgnoreCaseEquals("0") ? null : new DateTimeOffset(DateTime.Parse(data.SettleDate), TimeSpan.FromHours(-4)),
                    BetAmount = decimal.Parse(data.InitData.BetAmount),
                    ValidBetAmount = decimal.Parse(data.InitData.BetAmount),
                    PlayerWinLoss = data.InitData.Win.IgnoreCaseEquals("0") ? null : decimal.Parse(data.Result),
                    Commission = 0,
                    Turnover = data.InitData.Win.IgnoreCaseEquals("0") ? null : decimal.Parse(data.InitData.BetAmount),
                    SettleAmount = decimal.Parse(data.InitData.BetAmount) + decimal.Parse(data.Result),
                    TieBetAdjustment = null,

                    AdditionalData = JsonSerializer.Serialize(data),
                })
                .ToArray();
            return new() { Success = true, Datas = datas };
        }
    }
}
