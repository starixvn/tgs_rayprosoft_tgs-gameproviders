using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core.Extensions;
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

            (bool Success, TGS.GameProviders.Core.WGSports.Models.WGSportsBetInfo[]? BetInfos) = await _client.GetGameRecordAsync(
                configuration.ApiUrl,
                configuration.PartnerCode,
                configuration.PartnerKey,
                configuration.IV,
                (command.IsSettlement) ? GameRecordTime.SettleDate : GameRecordTime.BetDate,
                command.StartTime.ToOffset(TimeSpan.FromHours(-4)).ToString("yyyy-MM-dd HH:mm:ss"),
                command.EndTime.ToOffset(TimeSpan.FromHours(-4)).ToString("yyyy-MM-dd HH:mm:ss")).ConfigureAwait(false);

            if (!Success)
            {
                return new() { Success = false };
            }

            if (Success && BetInfos == null)
            {
                return new() { Success = true };
            }

            var gameProviderCode = _service.GetGameProviderCode();
            var gameProviderId = await _service.GetGameProviderIdAsync().ConfigureAwait(false);

            var datas = BetInfos
                .Where(data => data.Reference.IgnoreCaseNotContains("不接受"))
                .Select(data => new GameRecordListModel()
                {
                    GameProviderCode = gameProviderCode,
                    GameProviderId = gameProviderId,
                    PlayerCode = data.Username,
                    Status = data.InitData.Win.IgnoreCaseEquals("0") ? RoundStatus.Open : RoundStatus.Closed,
                    ClientStatus = data.InitData.Win.IgnoreCaseEquals("0") ? ClientRoundStatus.Open : _service.GetClientRound(data.Result),
                    ExternalRoundId = data.Reference,
                    ExternalGameId = "1",
                    ExternalCreatedOn = new DateTimeOffset(DateTime.Parse(data.RecordDate), TimeSpan.FromHours(-4)),
                    ExternalClosedOn = data.InitData.Win.IgnoreCaseEquals("0") ? null : new DateTimeOffset(DateTime.Parse(data.SettleDate), TimeSpan.FromHours(-4)),
                    BetAmount = _service.GetAmount(data.InitData.BetAmount) ?? 0,
                    ValidBetAmount = _service.GetAmount(data.InitData.BetAmount) ?? 0,
                    PlayerWinLoss = data.InitData.Win.IgnoreCaseEquals("0") ? null : _service.GetAmount(data.Result),
                    Commission = 0,
                    Turnover = data.InitData.Win.IgnoreCaseEquals("0") ? null : decimal.Parse(data.InitData.BetAmount),
                    SettleAmount = _service.GetAmount(data.InitData.BetAmount) ?? 0 + _service.GetAmount(data.Result) ?? 0,
                    TieBetAdjustment = null,

                    AdditionalData = JsonSerializer.Serialize(data),
                })
                .ToArray();
            return new() { Success = true, Datas = datas };
        }
    }
}
