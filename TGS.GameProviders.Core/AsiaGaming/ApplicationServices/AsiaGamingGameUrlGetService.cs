using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core.AsiaGaming.Models;
using TGS.GameProviders.Core.Extensions;

namespace TGS.GameProviders.Core.AsiaGaming.ApplicationServices
{
    public class AsiaGamingGameUrlGetService : IGetService<AsiaGamingGameUrlQuery, GameUrlResult>
    {
        private readonly ILogger<AsiaGamingGameUrlGetService> _logger;
        private readonly IAsiaGamingGamePlayer _playerService;
        private readonly IAsiaGamingService _service;
        private readonly IAsiaGamingGameSession _sessionService;
        private readonly IAsiaGamingUtility _utility;

        public AsiaGamingGameUrlGetService(
            ILogger<AsiaGamingGameUrlGetService> logger,
            IAsiaGamingService service,
            IAsiaGamingGamePlayer playerService,
            IAsiaGamingGameSession sessionService,
            IAsiaGamingUtility utility)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
            _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
            _utility = utility ?? throw new ArgumentNullException(nameof(utility));
        }

        public async ValueTask<GameUrlResult> GetAsync(
            AsiaGamingGameUrlQuery query,
            CancellationToken cancellationToken = default)
        {
            var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);

            var gameProviderId = await _service.GetGameProviderIdAsync().ConfigureAwait(false);
            var gameProviderCurrencyCode = await _service
                .GetGameProviderCurrencyCodeAsync(gameProviderId, query.CurrencyCode)
                .ConfigureAwait(false);

            var playerExist = await _playerService
                .CheckOrCreateAccountAsync(configuration, query.PlayerCode, gameProviderCurrencyCode)
                .ConfigureAwait(false);

            if (!playerExist)
            {
                return null;
            }

            var sessionCreated = await _sessionService
                .CreateGameSessionAsync(configuration, query.PlayerCode, query.LoginRecordId)
                .ConfigureAwait(false);

            if (!sessionCreated)
            {
                return null;
            }

            var brandId = await _service.GetBrandIdAsync(query.BrandCode).ConfigureAwait(false);
            var game = await _service.GetGameAsync(query.GameId!.Value).ConfigureAwait(false);
            var gameProviderLanguage = await _service
                .GetGameProviderLanguageCodeAsync(gameProviderId, query.Language).ConfigureAwait(false);

            var domain = await _service.GetGameDomainAsync(brandId, gameProviderId, query.PlatformType)
                .ConfigureAwait(false);

            var data = new AsiaGamingGameUrlRequest
            {
                AgentCode = configuration.AgentCode,
                PlayerCode = query.PlayerCode,
                Sid = $"{configuration.AgentCode}{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}",
                Lang = gameProviderLanguage,
                GameId = game.GameExternalId,
                ReturnUrl = query.BackUrl.AbsoluteUri,
                Currency = gameProviderCurrencyCode
            };

            var queryString = _utility.GetQueryString(data, "/\\\\/");
            var encodedParas = _utility.DesEncrypt(queryString, configuration.DesKey);
            var key = _utility.Md5Hash($"{encodedParas}{configuration.Md5Key}");

            var url = domain.AppendQueryString(game.EndpointPath)
                .Replace("{PARAS}", encodedParas)
                .Replace("{KEY}", key);

            return new GameUrlResult
            {
                Url = new Uri(url)
            };
        }
    }
}