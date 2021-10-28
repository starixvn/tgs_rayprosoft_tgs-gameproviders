using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using TGS.GameProviders.ApiClients;
using TGS.GameProviders.ApiClients.WGSports;
using TGS.GameProviders.ApiClients.WGSports.Models;

namespace TGS.GameProviders.Core.WGSports.ApplicationServices
{
    public class WGSportsPlayerFundInGetService : IGetService<WGSportsPlayerFundInQuery, PlayerFundResult>
    {
        private readonly IWGSportsApiClient _client;
        private readonly ILogger<WGSportsPlayerFundInGetService> _logger;
        private readonly IWGSportsService _service;

        public WGSportsPlayerFundInGetService(
            ILogger<WGSportsPlayerFundInGetService> logger,
            IWGSportsApiClient client,
            IWGSportsService service)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async ValueTask<PlayerFundResult> GetAsync(WGSportsPlayerFundInQuery command, CancellationToken cancellationToken = default)
        {
            var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);

            var source = new WGSportsAccessMoneyRequest()
            {
                PlayerCode = command.PlayerCode,
                AmountType = WGSportsAmountTypes.FundIn,
                OrderId = command.OrderId,
                Amount = command.Amount.ToString()
            };
            var response = await _client.AccessMoneyAsync(
                configuration.ApiUrl,
                configuration.PartnerCode,
                configuration.PartnerKey,
                configuration.IV,
                source).ConfigureAwait(false);

            return (response != null && response?.ReturnCode == 1) ? new() { OrderId = command.OrderId } : null;
        }
    }
}
