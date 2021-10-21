using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.ApiClients;
using TGS.GameProviders.ApiClients.WGSports.Models;
using TGS.ServiceUtility;

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

            var prepareLoginRequest = new WGSportsPrepareLoginRequest()
            {
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                PlayerCode = command.PlayerCode,
                PlayerNickname = command.PlayerCode,
                CurrencyCode = currencyCode
            };

            var prepareLoginResponse = await _client.GetPrepareLoginAsync(
                configuration.ApiUrl,
                configuration.PartnerCode,
                configuration.PartnerKey,
                configuration.IV,
                prepareLoginRequest)
                .ConfigureAwait(false);

            if (prepareLoginResponse.ReturnCode is not 1)
            {
                return null;
            }

            var sportsLoginRequest = new WGSportsLoginRequest()
            {
                Token = prepareLoginResponse.Token,
                Lang = cultureCode
            };

            var loginResponse = await _client.GetLoginAsync(
                configuration.ApiUrl,
                configuration.PartnerCode,
                sportsLoginRequest).ConfigureAwait(false);

            return prepareLoginResponse.ReturnCode is not 1 ? null : (new() { Url = new(loginResponse.LoginUrl) });
        }
    }
}
