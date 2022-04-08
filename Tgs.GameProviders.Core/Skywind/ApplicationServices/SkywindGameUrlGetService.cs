using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.Core.Infrastructures.Security;
using Tgs.GameProviders.Core.Skywind.Models;
using Tgs.GameProviders.QueryServer.Models;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.Skywind.ApplicationServices
{
    public class SkywindGameUrlGetService : IGetService<SkywindGameUrlQuery, GameUrlResult>
    {
        private readonly ISkywindApiClient _client;
        private readonly IGameService _gameService;
        private readonly ILogger<SkywindGameUrlGetService> _logger;
        private readonly ISkywindService _service;
        private readonly ISkywindAccessToken _skywindAccessToken;
        private readonly ITokenProvider _tokenProvider;

        public SkywindGameUrlGetService(
            ISkywindApiClient client,
            IGameService gameService,
            ILogger<SkywindGameUrlGetService> logger,
            ISkywindService service,
            ITokenProvider tokenProvider,
            ISkywindAccessToken skywindAccessToken
            )
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _gameService = gameService ?? throw new ArgumentNullException(nameof(gameService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
            _skywindAccessToken = skywindAccessToken ?? throw new ArgumentNullException(nameof(skywindAccessToken));
        }

        public async ValueTask<GameUrlResult> GetAsync(SkywindGameUrlQuery command, CancellationToken cancellationToken = default)
        {
            var encryptedToken = await _tokenProvider.EncryptAsync(command.LoginRecordId).ConfigureAwait(false);
            var gameProviderCultureCode = await _service.GetGameProviderCultureCodeAsync(command.Language).ConfigureAwait(false);
            var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);
            var getGame = await _gameService.GetCacheAsync(command.GameId.Value).ConfigureAwait(false);

            if (getGame == null)
            {
                return null;
            }

            string url;

            if (getGame.GameType == GameType.LiveDealerLobby)
            {
                url = $"{configuration.LiveLobbyUrl}#/ticket/{encryptedToken}?lang={gameProviderCultureCode}";
            }
            else
            {
                var data = new SkywindGameUrlRequest
                {
                    PlayerCode = command.PlayerCode,
                    GameCode = getGame.GameExternalId,
                    Ticket = encryptedToken,
                    Language = gameProviderCultureCode
                };

                var getaccessToken = await _skywindAccessToken.GetAccessTokenAsync().ConfigureAwait(false);

                var responseGameUrl = await _client.GetGameUrlAsync(
                    configuration.ApiUrl,
                    getaccessToken,
                    data).ConfigureAwait(false);

                if (responseGameUrl?.Code != 0)
                {
                    return null;
                }

                url = responseGameUrl.Url;
            }

            return new()
            {
                Url = new(url)
            };
        }
    }
}
