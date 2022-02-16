using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.Core.Infrastructures.Security;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core.Extensions;

namespace TGS.GameProviders.Core.Habanero.ApplicationServices
{
    internal class HabaneroGameUrlGetService : IGetService<HabaneroGameUrlQuery, GameUrlResult>
    {
        private readonly ILogger<HabaneroGameUrlGetService> _logger;
        private readonly IHabaneroService _service;
        private readonly ITokenProvider _tokenProvider;

        public HabaneroGameUrlGetService(
            ILogger<HabaneroGameUrlGetService> logger,
            IHabaneroService service,
            ITokenProvider tokenProvider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
        }

        public async ValueTask<GameUrlResult> GetAsync(
            HabaneroGameUrlQuery query,
            CancellationToken cancellationToken = default)
        {
            var encryptedToken = await _tokenProvider
                .EncryptAsync(query.LoginRecordId)
                .ConfigureAwait(false);

            var configuration = await _service
                .GetSettingConfigurationAsync()
                .ConfigureAwait(false);

            var player = await _service.GetPlayerAsync(query.PlayerCode).ConfigureAwait(false);
            var game = await _service.GetGameAsync(query.GameId!.Value).ConfigureAwait(false);

            var domain = await _service
                .GetGameDomainAsync(player.BrandId, game.GameProviderId, query.PlatformType)
                .ConfigureAwait(false);

            var gameProviderCultureCode = await _service
                .GetGameProviderLanguageCodeAsync(game.GameProviderId, query.Language)
                .ConfigureAwait(false);

            var url = domain
                .AppendQueryString(game.EndpointPath)
                .AppendQueryString($"keyname={game.GameExternalId}")
                .Replace("{BRANDID}", configuration.BrandId)
                .Replace("{TOKEN}", encryptedToken)
                .Replace("{CULTURE}", gameProviderCultureCode);

            return new GameUrlResult
            {
                Url = new Uri(url)
            };
        }
    }
}
