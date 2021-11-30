using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core.Extensions;
using TGS.GameProviders.Core.Saba.Models;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Core.Saba.ApplicationServices
{
    internal class SabaGameUrlGetService : IGetService<SabaGameUrlQuery, GameUrlResult>
    {
        private readonly ILogger<SabaGameUrlGetService> _logger;
        private readonly ISabaService _service;
        private readonly ISabaGamePlayer _playerService;
        private readonly ISabaApiClient _client;

        public SabaGameUrlGetService(
            ILogger<SabaGameUrlGetService> logger,
            ISabaService service,
            ISabaGamePlayer playerService,
            ISabaApiClient client)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async ValueTask<GameUrlResult> GetAsync(SabaGameUrlQuery query, CancellationToken cancellationToken = default)
        {
            var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);
            var gameProviderId = await _service.GetGameProviderIdAsync().ConfigureAwait(false);

            var response = await GetGameUrlAsync(gameProviderId, configuration, query)
                .ConfigureAwait(false);

            if (response is null || response.Code is not SabaGetGameUrlApiError.Ok)
            {
                return null;
            }

            var cultureCode = await _service
                .GetGameProviderLanguageCodeAsync(gameProviderId, query.Language)
                .ConfigureAwait(false);

            var url = response.Url
                .AppendQueryString($"tsId={query.LoginRecordId}")
                .AppendQueryString($"lang={cultureCode}");

            return new GameUrlResult
            {
                Url = new Uri(url)
            };
        }

        private async ValueTask<SabaGameUrlResponse> GetGameUrlAsync(Guid gameProviderId, SabaConfiguration configuration, SabaGameUrlQuery query)
        {
            var isTestEnv = _service.IsTestEnvironment(configuration.TestEnvironments, query.EnvironmentName);

            var playerCode = query.PlayerCode;
            var playerId = isTestEnv ? $"{playerCode}_test" : playerCode;

            var platform = query.PlatformType == PlatformType.Mobile ? "2" : "1";

            var request = new SabaGameUrlRequest
            {
                VendorId = configuration.VendorId,
                Platform = platform,
                PlayerId = playerId
            };

            var response = await _client.GetGameUrlAsync(configuration.ApiUrl, request).ConfigureAwait(false);

            if (response.Code is SabaGetGameUrlApiError.Ok or not SabaGetGameUrlApiError.MemberNotExist)
            {
                return response;
            }

            var created = await _playerService
                .CreateMemberAsync(gameProviderId, configuration, playerId, query.CurrencyCode, isTestEnv)
                .ConfigureAwait(false);

            return !created ?
                null :
                await _client.GetGameUrlAsync(configuration.ApiUrl, request).ConfigureAwait(false);
        }
    }
}