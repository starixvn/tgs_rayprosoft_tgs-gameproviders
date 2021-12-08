using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;

namespace TGS.GameProviders.Core.WGSports.ApplicationServices
{
    public class WGSportsGameUrlGetService : IGetService<WGSportsGameUrlQuery, GameUrlResult>
    {
        private readonly IWGSportsApiClient _client;
        private readonly ILogger<WGSportsGameUrlGetService> _logger;
        private readonly IWGSportsService _service;

        public WGSportsGameUrlGetService(
            ILogger<WGSportsGameUrlGetService> logger,
            IWGSportsApiClient client,
            IWGSportsService service)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async ValueTask<GameUrlResult> GetAsync(WGSportsGameUrlQuery command, CancellationToken cancellationToken = default)
        {
            var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);
            var cultureCode = await _service.GetGameProviderCultureCodeAsync(command.Language).ConfigureAwait(false);
            var currencyCode = await _service.GetGameProviderCurrencyCodeAsync(command.CurrencyCode).ConfigureAwait(false);

            var token = await _client.GetPrepareLoginAsync(
                configuration.ApiUrl,
                configuration.PartnerCode,
                configuration.PartnerKey,
                configuration.IV,
                DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                command.PlayerCode,
                command.PlayerCode,
                currencyCode,
                string.Empty).ConfigureAwait(false);

            if (token == null)
            {
                return null;
            }

            var gameUrl = await _client.GetLoginAsync(
                configuration.ApiUrl,
                configuration.PartnerCode,
                token,
                cultureCode).ConfigureAwait(false);

            return gameUrl == null ? null : (new() { Url = gameUrl });
        }
    }
}
