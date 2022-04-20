using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.QueryServer.Models;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.WGSports.ApplicationServices
{
    public class WGSportsGameRecordListGetService : IGetService<WGSportsGameRecordListQuery, GameRecordListResult>
    {
        private readonly IWGSportsApiClient _client;
        private readonly IWGSportsService _service;

        public WGSportsGameRecordListGetService(
            IWGSportsApiClient client,
            IWGSportsService service
            )
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async ValueTask<GameRecordListResult> GetAsync(WGSportsGameRecordListQuery command, CancellationToken cancellationToken = default)
        {
            var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);

            (bool Success, Tgs.GameProviders.Core.WGSports.Models.WGSportsBetInfo[]? BetInfos) = await _client.GetGameRecordAsync(
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

            var gameProviderCode = GameProviderType.WGSports;
            var gameProviderId = await _service.GetGameProviderIdAsync().ConfigureAwait(false);

            var datas = BetInfos
                .Where(data => IgnoreCaseNotContains(data.Reference, "不接受") && IgnoreCaseNotContains(data.Reference, "等待确认"))
                .Select(data => new GameRecordListModel()
                {
                    GameProviderCode = gameProviderCode,
                    GameProviderId = gameProviderId,
                    PlayerCode = data.Username,
                    Status = IgnoreCaseEquals(data.InitData.Win) ? RoundStatus.Open : RoundStatus.Closed,
                    ClientStatus = IgnoreCaseEquals(data.InitData.Win) ? RoundClientStatus.Open : _service.GetClientRound(data.Result),
                    ExternalRoundId = data.Reference,
                    ExternalGameId = "1",
                    ExternalCreatedOn = new DateTimeOffset(DateTime.Parse(data.RecordDate), TimeSpan.FromHours(-4)),
                    ExternalClosedOn = IgnoreCaseEquals(data.InitData.Win) ? null : new DateTimeOffset(DateTime.Parse(data.SettleDate), TimeSpan.FromHours(-4)),
                    BetAmount = _service.GetAmount(data.InitData.BetAmount),
                    ValidBetAmount = _service.GetAmount(data.InitData.BetAmount),
                    PlayerWinLoss = IgnoreCaseEquals(data.InitData.Win) ? null : _service.GetAmount(data.Result),
                    Commission = 0,
                    Turnover = IgnoreCaseEquals(data.InitData.Win) ? null : decimal.Parse(data.InitData.BetAmount),
                    SettleAmount = IgnoreCaseEquals(data.InitData.Win) ? null : _service.GetAmount(data.InitData.BetAmount) + _service.GetAmount(data.Result),
                    TieBetAdjustment = null,

                    AdditionalData = JsonSerializer.Serialize(data),
                })
                .ToArray();
            return new() { Success = true, Datas = datas };
        }

        private bool IgnoreCaseEquals(string value)
             => string.Equals(value, "0", StringComparison.OrdinalIgnoreCase);

        private bool IgnoreCaseNotContains(string source, string trigger)
            => !source.Contains(trigger, StringComparison.OrdinalIgnoreCase);
    }
}
