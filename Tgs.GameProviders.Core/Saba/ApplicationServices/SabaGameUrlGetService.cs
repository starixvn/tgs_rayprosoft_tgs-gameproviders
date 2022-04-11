using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.GameProviders.Core.Extensions;
using Tgs.GameProviders.Core.Saba.Models;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.Saba.ApplicationServices
{
    internal class SabaGameUrlGetService : IGetService<SabaGameUrlQuery, GameUrlResult>
    {
        private readonly ISabaApiClient _client;
        private readonly ILogger<SabaGameUrlGetService> _logger;
        private readonly ISabaGamePlayer _playerService;
        private readonly ISabaService _service;
        private readonly ISabaUtility _utility;

        public SabaGameUrlGetService(
            ILogger<SabaGameUrlGetService> logger,
            ISabaService service,
            ISabaGamePlayer playerService,
            ISabaUtility utility,
            ISabaApiClient client)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
            _utility = utility ?? throw new ArgumentNullException(nameof(utility));
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async ValueTask<GameUrlResult> GetAsync(SabaGameUrlQuery query, CancellationToken cancellationToken = default)
        {
            var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);
            var gameProviderId = await _service.GetGameProviderIdAsync().ConfigureAwait(false);

            var response = await GetGameUrlAsync(gameProviderId, configuration, query)
                .ConfigureAwait(false);

            if (response?.ErrorCode is not SabaGetGameUrlApiError.Ok)
            {
                return null;
            }

            var culture = await _service
                .GetGameProviderLanguageCodeAsync(gameProviderId, query.Language)
                .ConfigureAwait(false);

            var url = response.Url
                .AppendQueryString($"tsId={query.LoginRecordId}")
                .AppendQueryString($"lang={culture}");

            return new GameUrlResult
            {
                Url = new Uri(url)
            };
        }

        private async ValueTask<SabaGameUrlResponse> GetGameUrlAsync(Guid gameProviderId, SabaConfiguration configuration, SabaGameUrlQuery query)
        {
            var isTestEnv = configuration.IsTestEnvironment;

            var playerCode = query.PlayerCode;
            var playerId = _utility.ToSabaPlayerId(playerCode, isTestEnv);

            var platform = query.PlatformType == PlatformType.Mobile ? "2" : "1";

            var request = new SabaGameUrlRequest
            {
                VendorId = configuration.VendorId,
                Platform = platform,
                PlayerId = playerId
            };

            var response = await _client.GetGameUrlAsync(configuration.ApiUrl, request).ConfigureAwait(false);

            if (response.ErrorCode is SabaGetGameUrlApiError.Ok or not SabaGetGameUrlApiError.MemberNotExist)
            {
                return response;
            }

            var created = await _playerService
                .CreateMemberAsync(gameProviderId, configuration, playerId, query.CurrencyCode)
                .ConfigureAwait(false);

            return !created ?
                null :
                await _client.GetGameUrlAsync(configuration.ApiUrl, request).ConfigureAwait(false);
        }
    }
}
