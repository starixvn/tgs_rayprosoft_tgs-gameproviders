using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.Core.Infrastructures.Security;
using TGS.GameProviders.ApiClients;
using TGS.GameProviders.ApiClients.SexyGaming;
using TGS.GameProviders.ApiClients.SexyGaming.Models;
using TGS.GameProviders.Domains.Extensions;
using TGS.GameProviders.Repositories;
using TGS.GameProviders.Repositories.Domains;
using TGS.ServiceUtility;

namespace TGS.GameProviders.Core.SexyGaming.ApplicationServices
{
    public class SexyGamingGameUrlGetService : IGetService<SexyGamingGameUrlQuery, GameUrlResult>
    {
        private readonly ISexyGamingApiClient _client;
        private readonly IGameRepository _gameRepository;
        private readonly ISexyGamingGamePlayer _gamingGamePlayerCreate;
        private readonly ILogger<SexyGamingGameUrlGetService> _logger;
        private readonly ISexyGamingService _service;
        private readonly ITokenProvider _tokenProvider;

        public SexyGamingGameUrlGetService(
            ISexyGamingApiClient client,
            IGameRepository gameRepository,
            ILogger<SexyGamingGameUrlGetService> logger,
            ISexyGamingService service,
            ITokenProvider tokenProvider,
            ISexyGamingGamePlayer gamingGamePlayerCreate
            )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
            _gamingGamePlayerCreate = gamingGamePlayerCreate ?? throw new ArgumentNullException(nameof(gamingGamePlayerCreate));
        }

        public async ValueTask<GameUrlResult> GetAsync(SexyGamingGameUrlQuery command, CancellationToken cancellationToken = default)
        {
            var encryptedToken = await _tokenProvider.EncryptAsync(command.LoginRecordId).ConfigureAwait(false);
            var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);
            var gameProviderCulture = await _service.GetGameProviderCultureCodeAsync(command.Language).ConfigureAwait(false);
            var gameProviderCurrency = await _service.GetGameProviderCurrencyCodeAsync(command.CurrencyCode).ConfigureAwait(false);

            if (configuration == null)
            {
                return null;
            }

            var data = new SexyGamingGameUrlRequest
            {
                UserId = command.PlayerCode,
                IsMobileLogin = command.PlatformType == PlatformType.Mobile,
                Language = gameProviderCulture,
                Extension1 = JsonSerializer.Serialize(new
                {
                    token = encryptedToken
                })
            };

            var url = default(string);
            var responseGameUrl = await _client.GetGamingGameUrlAsync(configuration.ApiUrl, data).ConfigureAwait(false);

            if (responseGameUrl.Status.IgnoreCaseNotEquals(((int)SexyGamingGameApiError.Success).ToString("d4")))
            {
                if (responseGameUrl.Status.IgnoreCaseEquals(((int)SexyGamingGameApiError.AccountIsNotExists).ToString("d4")))
                {
                    if (await _gamingGamePlayerCreate.CreatePlayerAsync(configuration, command.PlayerCode, command.CurrencyCode, gameProviderCurrency, gameProviderCulture))
                    {
                        responseGameUrl = await _client.GetGamingGameUrlAsync(configuration.ApiUrl, data).ConfigureAwait(false);
                    }
                }
            }

            return responseGameUrl.Status.IgnoreCaseEquals(((int)SexyGamingGameApiError.Success).ToString("d4")) ? new() { Url = new Uri(responseGameUrl.Url) } : null;
        }
    }
}
