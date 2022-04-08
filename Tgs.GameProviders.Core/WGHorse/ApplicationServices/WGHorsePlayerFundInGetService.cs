using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.WGHorse.ApplicationServices
{
    public class WGHorsePlayerFundInGetService : IGetService<WGHorsePlayerFundInQuery, PlayerFundResult>
    {
        private readonly IWGHorseApiClient _client;
        private readonly ILogger<WGHorsePlayerFundInGetService> _logger;
        private readonly IWGHorseService _service;

        public WGHorsePlayerFundInGetService(
            ILogger<WGHorsePlayerFundInGetService> logger,
            IWGHorseApiClient client,
            IWGHorseService service)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async ValueTask<PlayerFundResult> GetAsync(WGHorsePlayerFundInQuery command, CancellationToken cancellationToken = default)
        {
            var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);

            var response = await _client.AccessMoneyFundInAsync(
                configuration.ApiUrl,
                configuration.PartnerCode,
                configuration.PartnerKey,
                configuration.IV,
                command.PlayerCode,
                command.OrderId,
                command.Amount).ConfigureAwait(false);

            return response ? new() { OrderId = command.OrderId } : null;
        }
    }
}
