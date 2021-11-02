using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using TGS.GameProviders.ApiClients;
using TGS.GameProviders.ApiClients.VR.Models;
using TGS.GameProviders.Repositories.Domains;

namespace TGS.GameProviders.Core.VR.ApplicationServices
{
    public class VRGameRecordListGetService : IGetService<VRGameRecordListQuery, GameRecordListResult>
    {
        private readonly IVRApiClient _client;
        private readonly ILogger<VRGameRecordListGetService> _logger;
        private readonly IVRService _service;

        public VRGameRecordListGetService(
            IVRApiClient client,
            ILogger<VRGameRecordListGetService> logger,
            IVRService service
            )
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async ValueTask<GameRecordListResult> GetAsync(VRGameRecordListQuery command, CancellationToken cancellationToken = default)
        {
            var brandMerchant = await _service.GetBrandMerchantAsync(command.BrandCode).ConfigureAwait(false);
            if (brandMerchant == null)
            {
                return new()
                {
                    Success = false
                };
            }

            // VR預設Page從0開始
            var data = new VRGameRecordRequest
            {
                StartTime = command.StartTime.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                EndTime = command.EndTime.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                IsUpdateTime = command.IsUpdateTime,
                Page = command.Page - 1,
                Rows = command.Rows
            };

            var response = await _client.GetGameRecordAsync(
                brandMerchant.ApiUrl,
                brandMerchant.Merchantid,
                brandMerchant.Version,
                brandMerchant.Deskey,
                data);

            if (response == null)
            {
                return new() { Success = false };
            }

            if (response?.Records == null || response?.Records.Length == 0)
            {
                return new() { Success = true };
            }

            var datas = response.Records.Select(data =>
               new GameRecordListModle()
               {
                   ExternalRoundId = data.SerialNumber,
                   PlayerCode = data.PlayerName,
                   ExternalGameId = data.ChannelId.ToString(),
                   Status = _service.GetRoundStatus(data.State),
                   ExternalCreatedOn = data.CreateTime.ToOffset(TimeSpan.FromHours(+8)),
                   ExternalClosedOn = data.UpdateTime.HasValue ? data.UpdateTime.Value.ToOffset(TimeSpan.FromHours(+8)) : null,
                   BetAmount = data.Cost,
                   ValidBetAmount = data.State == 1 ? 0 : data.Cost,
                   PlayerWinLoss = data.State == 1
                               ? 0
                               :
                               _service.GetRoundStatus(data.State) == RoundStatus.Closed
                                   ?
                                   data.PlayerPrize - data.Cost
                                   : null,
                   Commission = 0,
                   Turnover = data.State == 1 ? 0 : data.Cost,
                   SettleAmount = data.PlayerPrize,
                   TieBetAdjustment = null,
                   ClientStatus = _service.GetClientRound(data.State),
                   AdditionalData = JsonSerializer.Serialize(data),
               })
               .ToArray();

            return new() { Success = true, Datas = datas };
        }
    }
}
