using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.GameProviders.Core.Saba;
using Tgs.GameProviders.Core.Saba.Models;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Services.Saba
{
    internal class SabaGamePlayer : ISabaGamePlayer
    {
        private readonly ISabaApiClient _client;
        private readonly ILogger<SabaGamePlayer> _logger;
        private readonly ISabaService _service;

        public SabaGamePlayer(
            ILogger<SabaGamePlayer> logger,
            ISabaService service,
            ISabaApiClient client)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async ValueTask<bool> CreateMemberAsync(
            Guid gameProviderId,
            SabaConfiguration configuration,
            string sabaPlayerId,
            string currencyCode)
        {
            var oddsType = GetOddsType(currencyCode);
            var gameProviderCurrencyCode = configuration.IsTestEnvironment ?
                "20" :
                await _service
                    .GetGameProviderCurrencyCodeAsync(gameProviderId, currencyCode)
                    .ConfigureAwait(false);

            var request = new SabaCreateMemberRequest
            {
                OperatorId = configuration.OperatorId,
                VendorId = configuration.VendorId,
                PlayerId = sabaPlayerId,
                Username = sabaPlayerId,
                CurrencyCode = gameProviderCurrencyCode,
                OddsType = oddsType
            };

            var response = await _client.CreateMemberAsync(configuration.ApiUrl, request).ConfigureAwait(false);

            return response.ErrorCode == SabaCreateMemberApiError.Ok;
        }

        private string GetOddsType(string currencyCode)
        {
            var oddType = currencyCode switch
            {
                "USD" => SabaOddsType.American,
                "VND_1000" => SabaOddsType.Malay,
                "IDR_1000" => SabaOddsType.Indo,
                "KRW" => SabaOddsType.Decimal,
                "MYR" => SabaOddsType.Malay,
                "THB" => SabaOddsType.Malay,
                "GBP" => SabaOddsType.Decimal,
                "JPY" => SabaOddsType.Decimal,
                "CNY" => SabaOddsType.China,
                _ => SabaOddsType.Malay
            };

            return ((int)oddType).ToString();
        }
    }
}
