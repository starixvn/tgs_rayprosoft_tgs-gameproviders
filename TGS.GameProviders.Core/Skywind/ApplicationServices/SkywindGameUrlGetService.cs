using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.Core.Infrastructures.Security;
using TGS.GameProviders.ApiClients;
using TGS.GameProviders.ApiClients.Skywind.Models;
using TGS.GameProviders.Repositories;
using TGS.GameProviders.Repositories.Domains;
using TGS.ServiceUtility;

namespace TGS.GameProviders.Core.Skywind.ApplicationServices
{
    public class SkywindGameUrlGetService : IGetService<SkywindGameUrlQuery, GameUrlResult>
    {
        private readonly ISkywindApiClient _client;
        private readonly IGameRepository _gameRepository;
        private readonly ILogger<SkywindGameUrlGetService> _logger;
        private readonly ISkywindService _service;
        private readonly ISkywindAccessToken _skywindAccessToken;
        private readonly ITokenProvider _tokenProvider;

        public SkywindGameUrlGetService(
            ISkywindApiClient client,
            IGameRepository gameRepository,
            ILogger<SkywindGameUrlGetService> logger,
            ISkywindService service,
            ITokenProvider tokenProvider,
            ISkywindAccessToken skywindAccessToken
            )
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
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
            var getGame = await _gameRepository.GetGameAsync(command.GameId.Value).ConfigureAwait(false);

            if (getGame == null)
            {
                return null;
            }

            string url;

            if (getGame.Type == GameType.LiveDealerLobby)
            {
                url = $"{configuration.LiveLobbyUrl}#/ticket/{encryptedToken}?lang={gameProviderCultureCode}";
            }
            else
            {
                var data = new SkywindGameUrlRequest
                {
                    PlayerCode = command.PlayerCode,
                    GameCode = getGame.ExternalId,
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
