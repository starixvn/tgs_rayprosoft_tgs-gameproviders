using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.ApiClients;
using TGS.GameProviders.ApiClients.WGSports;
using TGS.GameProviders.ApiClients.WGSports.Models;
using TGS.ServiceUtility;

namespace TGS.GameProviders.Core.WGSports.ApplicationServices
{
    public class WGSportsPlayerFundOutGetService : IGetService<WGSportsPlayerFundOutQuery, PlayerFundResult>
    {
        private readonly IWGSportsApiClient _client;
        private readonly ILogger<WGSportsPlayerFundOutGetService> _logger;
        private readonly IWGSportsService _service;

        public WGSportsPlayerFundOutGetService(
            ILogger<WGSportsPlayerFundOutGetService> logger,
            IWGSportsApiClient client,
            IWGSportsService service)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async ValueTask<PlayerFundResult> GetAsync(WGSportsPlayerFundOutQuery command, CancellationToken cancellationToken = default)
        {
            var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);

            var source = new WGSportsAccessMoneyRequest()
            {
                PlayerCode = command.PlayerCode,
                AmountType = WGSportsAmountTypes.FundOut,
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
