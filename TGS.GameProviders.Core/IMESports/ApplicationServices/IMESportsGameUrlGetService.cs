using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core.IMESports.Models;
using TGS.GameProviders.Core.Repositories.Models.Game;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.SettingConfiguration;
using static TGS.GameProviders.Core.IMESports.Models.IMESportsGameApiError;

namespace TGS.GameProviders.Core.IMESports.ApplicationServices
{
    internal class IMESportsGameUrlGetService : IGetService<IMESportsGameUrlQuery, GameUrlResult>
    {
        private readonly IIMESportsApiClient _client;
        private readonly IIMESportsDataStore _dataStore;
        private readonly ILogger<IMESportsGameUrlGetService> _logger;
        private readonly IIMESportsMerchantMapper _merchantMapper;
        private readonly IIMESportsGamePlayer _playerService;
        private readonly IIMESportsService _service;
        private readonly IIMESportsUtility _utility;

        public IMESportsGameUrlGetService(
            ILogger<IMESportsGameUrlGetService> logger,
            IIMESportsService service,
            IIMESportsGamePlayer playerService,
            IIMESportsMerchantMapper merchantMapper,
            IIMESportsUtility utility,
            IIMESportsDataStore dataStore,
            IIMESportsApiClient client)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
            _merchantMapper = merchantMapper ?? throw new ArgumentNullException(nameof(merchantMapper));
            _utility = utility ?? throw new ArgumentNullException(nameof(utility));
            _dataStore = dataStore ?? throw new ArgumentNullException(nameof(dataStore));
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async ValueTask<GameUrlResult> GetAsync(
            IMESportsGameUrlQuery query,
            CancellationToken cancellationToken = default)
        {
            var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);

            var game = await _service.GetGameAsync(query.GameId!.Value).ConfigureAwait(false);

            var response = await LaunchGameAsync(query, configuration, game)
                .ConfigureAwait(false);

            if (response is null || response.ErrorCode is not Success)
            {
                return null;
            }

            await _dataStore.CacheSessionTokenAsync(
                    query.PlayerCode,
                    game.GameExternalId,
                    response.SessionToken,
                    response.TimeToLeave)
                .ConfigureAwait(false);

            return new GameUrlResult
            {
                Url = new Uri(response.GameUrl)
            };
        }

        private async ValueTask<IMESportsLaunchGameResponse> LaunchGameAsync(
            IMESportsGameUrlQuery query,
            IMESportsConfiguration configuration,
            GameModel game)
        {
            var gameProviderId = await _service.GetGameProviderIdAsync().ConfigureAwait(false);
            var cultureCode = await _service.GetGameProviderLanguageCodeAsync(gameProviderId, query.Language).ConfigureAwait(false);

            var merchant = _merchantMapper.GetBrandMerchant(configuration.BrandMerchants, query.BrandCode);
            var imesPlayerId = _utility.ToIMESportsPlayerId(query.PlayerCode, merchant.Prefix);

            var launchRequest = new IMESportsLaunchGameRequest
            {
                MerchantCode = merchant.MerchantCode,
                IMESPlayerId = imesPlayerId,
                GameId = game.GameExternalId,
                Language = cultureCode,
                ProductWallet = configuration.ProductWallet,
                Http = configuration.SupportHttps ? "1" : "0"
            };

            var response = await LaunchGameAsync(merchant.ApiUrl, query.PlatformType, launchRequest)
                .ConfigureAwait(false);

            if (response.ErrorCode is Success or not PlayerDoesNotExist)
            {
                return response;
            }

            var gameProviderCurrencyCode = await _service
                .GetGameProviderCurrencyCodeAsync(gameProviderId, query.CurrencyCode)
                .ConfigureAwait(false);

            var created = await _playerService
                .CreatePlayerAsync(merchant, imesPlayerId, gameProviderCurrencyCode)
                .ConfigureAwait(false);

            return !created ?
                null :
                await LaunchGameAsync(merchant.ApiUrl, query.PlatformType, launchRequest)
                .ConfigureAwait(false);
        }

        private ValueTask<IMESportsLaunchGameResponse> LaunchGameAsync(Uri apiUrl, PlatformType platformType, IMESportsLaunchGameRequest request)
            => platformType == PlatformType.Mobile ?
                _client.LaunchMobileGameAsync(apiUrl, request) :
                _client.LaunchDesktopGameAsync(apiUrl, request);
    }
}