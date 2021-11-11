using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core.Extensions;
using TGS.GameProviders.Core.WGHorse.Models;
using TGS.GameProviders.Domains;

namespace TGS.GameProviders.Core.WGHorse.ApplicationServices
{
    public class WGHorseGameRecordListGetService : IGetService<WGHorseGameRecordListQuery, GameRecordListResult>
    {
        private readonly IWGHorseApiClient _client;
        private readonly ILogger<WGHorseGameRecordListGetService> _logger;
        private readonly IWGHorseService _service;

        public WGHorseGameRecordListGetService(
            IWGHorseApiClient client,
            ILogger<WGHorseGameRecordListGetService> logger,
            IWGHorseService service
            )
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async ValueTask<GameRecordListResult> GetAsync(WGHorseGameRecordListQuery command, CancellationToken cancellationToken = default)
        {
            var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);

            var data = new WGHorseGameRecordRequest
            {
                StartTime = command.StartTime,
                EndTime = command.EndTime,
                PageIndex = command.Page,
                PageSize = command.Rows
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

            if (response?.ReturnCode != 1)
            {
                if (response?.ReturnCode != 503)
                {
                    return new() { Success = false };
                }
            }

            if (command.Page.ToString().IgnoreCaseNotEquals(response.Page)
                || response.ArrayInvoice == null || response.ArrayInvoice.Length == 0)
            {
                return new() { Success = true };
            }

            var gameProviderCode = _service.GetGameProviderCode();
            var gameProviderId = await _service.GetGameProviderIdAsync().ConfigureAwait(false);

            var datas = response.ArrayInvoice.Select(data => new GameRecordListModle()
            {
                GameProviderCode = gameProviderCode,
                GameProviderId = gameProviderId,
                PlayerCode = data.MemberCode,
                Status = RoundStatus.Closed,
                ClientStatus = _service.GetClientRound(decimal.Parse(data.ShowWin)),
                ExternalRoundId = data.BatchNo,
                ExternalGameId = "1",
                ExternalCreatedOn = new DateTimeOffset(DateTime.Parse(data.RecordDate), TimeSpan.FromHours(+8)),
                ExternalClosedOn = new DateTimeOffset(DateTime.Parse(data.ShowDivideDate), TimeSpan.FromHours(+8)),
                BetAmount = decimal.Parse(data.ShowAmount),
                ValidBetAmount = decimal.Parse(data.ShowAmount),
                PlayerWinLoss = (data.ShowWin.IgnoreCaseContains("和局")) ? 0 : decimal.Parse(data.ShowWin),
                Commission = 0,
                Turnover = decimal.Parse(data.ShowAmount),
                SettleAmount = decimal.Parse(data.ShowAmount) + ((data.ShowWin.IgnoreCaseContains("和局")) ? 0m : decimal.Parse(data.ShowWin)),
                TieBetAdjustment = null,

                AdditionalData = JsonSerializer.Serialize(data),
            })
                .ToArray();
            return new() { Success = true, Datas = datas };
        }
    }
}
