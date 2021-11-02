using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using TGS.GameProviders.ApiClients;
using TGS.GameProviders.ApiClients.SBO.Models;
using TGS.GameProviders.Repositories.Domains;
using TGS.GameProviders.Repositories.Domains.Models.Game;
using TGS.GameProviders.Repositories.Domains.Models.SettingConfiguration;
using static TGS.GameProviders.ApiClients.SBO.Models.SBOApiErrorCode;

namespace TGS.GameProviders.Core.SBO.ApplicationServices
{
    public class SBOGameUrlGetService : IGetService<SBOGameUrlQuery, GameUrlResult>
    {
        private readonly ISBOApiClient _client;
        private readonly ILogger<SBOGameUrlGetService> _logger;
        private readonly ISBOGamePlayer _playerService;
        private readonly ISBOService _service;

        public SBOGameUrlGetService(
            ILogger<SBOGameUrlGetService> logger,
            ISBOService service,
            ISBOApiClient client,
            ISBOGamePlayer playerService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
        }

        public async ValueTask<GameUrlResult> GetAsync(SBOGameUrlQuery query,
            CancellationToken cancellationToken = default)
        {
            var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);

            var game = await _service.GetGameAsync(query.GameId.Value).ConfigureAwait(false);

            var response = await PlayerLoginAsync(configuration, query, game);

            if (response == null || response.Error.Code != None)
            {
                return null;
            }

            var gameProviderCulture =
                await _service.GetGameProviderCultureCodeAsync(query.Language).ConfigureAwait(false);

            var url = game.Type switch
            {
                GameType.VirtualSports or GameType.VirtualSportsLobby => $"{response.Url}&lang={gameProviderCulture}",
                GameType.Sportsbook => GetSportsbookUrl(query.PlatformType, response.Url, gameProviderCulture),
                _ => default
            };

            return new()
            {
                Url = new(url)
            };
        }

        private string GetSportsbookUrl(PlatformType platformType, string url, string cultureCode)
        {
            var queryString = "oddstyle=MY&theme=sbo&oddsmode=double";
            var device = platformType == PlatformType.Mobile ? "m" : "d";

            return $"{url}&device={device}&lang={cultureCode}&{queryString}";
        }

        private async ValueTask<SBOLoginResponse> PlayerLoginAsync(
                    SBOConfiguration configuration,
            SBOGameUrlQuery query,
            GetGame game)
        {
            var portfolio = _service.GetPortfolio(game.Type);
            var playerCode = query.PlayerCode;

            var loginRequest = new SBOLoginRequest
            {
                CompanyKey = configuration.CompanyKey,
                PlayerCode = playerCode,
                Portfolio = portfolio
            };

            var loginResponse = await _client.LoginAsync(configuration.ApiUrl, loginRequest).ConfigureAwait(false);
            if (loginResponse.Error.Code is None or not UserNotExist)
            {
                return loginResponse;
            }

            var created = await _playerService.CreatePlayerAsync(configuration, query.BrandCode, query.PlayerCode,
                query.CurrencyCode).ConfigureAwait(false);

            return !created ? null : await _client.LoginAsync(configuration.ApiUrl, loginRequest).ConfigureAwait(false);
        }
    }
}
