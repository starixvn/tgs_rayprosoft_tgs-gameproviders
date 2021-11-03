using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core.WGSports.Models;

namespace TGS.GameProviders.Core.WGSports.ApplicationServices
{
    public class WGSportsPlayerBalanceGetService : IGetService<WGSportsPlayerBalanceQuery, PlayerBalanceResult>
    {
        private readonly IWGSportsApiClient _client;
        private readonly ILogger<WGSportsPlayerBalanceGetService> _logger;
        private readonly IWGSportsGamePlayer _playerCreate;
        private readonly IWGSportsService _service;

        public WGSportsPlayerBalanceGetService(
            ILogger<WGSportsPlayerBalanceGetService> logger,
            IWGSportsApiClient client,
            IWGSportsService service,
            IWGSportsGamePlayer playerCreate
            )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _playerCreate = playerCreate ?? throw new ArgumentNullException(nameof(playerCreate));
        }

        public async ValueTask<PlayerBalanceResult> GetAsync(WGSportsPlayerBalanceQuery command, CancellationToken cancellationToken = default)
        {
            var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);
            var source = new WGSportsGetBalanceRequest()
            {
                PlayerCode = command.PlayerCode
            };
            var response = await _client.GetBalanceAsync(
                configuration.ApiUrl,
                configuration.PartnerCode,
                configuration.PartnerKey,
                configuration.IV,
                source).ConfigureAwait(false);

            if (response.ReturnCode == 602)
            {
                if (await _playerCreate.CreatePlayerAsync(configuration, command.PlayerCode, command.CurrencyCode).ConfigureAwait(false))
                {
                    response = await _client.GetBalanceAsync(
                        configuration.ApiUrl,
                        configuration.PartnerCode,
                        configuration.PartnerKey,
                        configuration.IV,
                        source).ConfigureAwait(false);
                }
            }

            return response.ReturnCode == 1 ? new() { Balance = decimal.Parse(response.Balance) } : null;
        }
    }
}
