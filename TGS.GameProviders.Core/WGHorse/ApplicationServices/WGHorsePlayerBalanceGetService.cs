using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.ApiClients;
using TGS.GameProviders.ApiClients.WGHorse.Models;
using TGS.ServiceUtility;

namespace TGS.GameProviders.Core.WGHorse.ApplicationServices
{
    public class WGHorsePlayerBalanceGetService : IGetService<WGHorsePlayerBalanceQuery, PlayerBalanceResult>
    {
        private readonly IWGHorseApiClient _client;
        private readonly ILogger<WGHorsePlayerBalanceGetService> _logger;
        private readonly IWGHorseGamePlayer _playerCreate;
        private readonly IWGHorseService _service;

        public WGHorsePlayerBalanceGetService(
            ILogger<WGHorsePlayerBalanceGetService> logger,
            IWGHorseApiClient client,
            IWGHorseService service,
            IWGHorseGamePlayer playerCreate
            )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _playerCreate = playerCreate ?? throw new ArgumentNullException(nameof(playerCreate));
        }

        public async ValueTask<PlayerBalanceResult> GetAsync(WGHorsePlayerBalanceQuery command, CancellationToken cancellationToken = default)
        {
            var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);
            var source = new WGHorseGetBalanceRequest()
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
