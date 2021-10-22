using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.Core.Infrastructures.Security;
using TGS.GameProviders.ApiClients;
using TGS.GameProviders.ApiClients.KY.Models;
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
        private readonly ITokenProvider _tokenProvider;

        public SkywindGameUrlGetService(
            ISkywindApiClient client,
            IGameRepository gameRepository,
            ILogger<SkywindGameUrlGetService> logger,
            ISkywindService service,
            ITokenProvider tokenProvider
            )
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
        }

        public async ValueTask<GameUrlResult> GetAsync(SkywindGameUrlQuery command, CancellationToken cancellationToken = default)
        {
            var encryptedToken = await _tokenProvider.EncryptAsync(command.LoginRecordId).ConfigureAwait(false);
            var cultureCode = await _service.GetGameProviderCultureCodeAsync(command.Language).ConfigureAwait(false);
            var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);

            var url = default(string);

            var getGame = await _gameRepository.GetGameAsync(command.GameId.Value).ConfigureAwait(false);

            if (getGame == null)
            {
                return null;
            }

            if (getGame.Type == GameType.LiveDealerLobby)
            {
                url = $"{configuration.LiveLobbyUrl}#/ticket/{encryptedToken}?lang={cultureCode}";
            }
            else
            {
                var data = new SkywindGameUrlRequest
                {
                    PlayerCode = command.PlayerCode,
                    GameCode = getGame.ExternalId,
                    Ticket = encryptedToken,
                    Language = cultureCode
                };

                var responseGameUrl = await _client.GetGameUrlAsync(
                    configuration.apiUrl,
                    configuration.getaccessToken, data).ConfigureAwait(false);

                if (responseGameUrl.Code.ToString().IgnoreCaseNotEquals(((int)GameApiError.Success).ToString()))
                {
                    throw new GameProviderApiErrorException(
                        _jsonSerializationProvider.SerializeToString(responseGameUrl));
                }

                url = responseGameUrl.Url;
            }

            return new()
            {
                Url = url
            };

            return (response.Data.Code == 0) ? new() { Url = new(response.Data.Url) } : null;
        }
    }
}
