using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core.Repositories.Models.Game;
using TGS.GameProviders.Core.SBO.Models;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.SettingConfiguration;
using static TGS.GameProviders.Core.SBO.Models.SBOApiErrorCode;

namespace TGS.GameProviders.Core.SBO.ApplicationServices
{
    public class SBOGameUrlGetService : IGetService<SBOGameUrlQuery, GameUrlResult>
    {
        private readonly ISBOApiClient _client;
        private readonly ILogger<SBOGameUrlGetService> _logger;
        private readonly ISBOGamePlayer _playerService;
        private readonly ISBOService _service;
        private readonly ISBOUtility _utility;

        public SBOGameUrlGetService(ILogger<SBOGameUrlGetService> logger,
            ISBOService service,
            ISBOGamePlayer playerService,
            ISBOUtility utility,
            ISBOApiClient client)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
            _utility = utility ?? throw new ArgumentNullException(nameof(utility));
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
                await _service.GetGameProviderCultureCodeAsync(game.GameProviderId, query.Language).ConfigureAwait(false);

            var url = game.GameType switch
            {
                GameType.VirtualSports or GameType.VirtualSportsLobby => $"{response.Url}&lang={gameProviderCulture}",
                GameType.Sportsbook => GetSportsbookUrl(query.PlatformType, response.Url, gameProviderCulture),
                _ => default
            };

            return url == null
                ? null
                : new GameUrlResult
                {
                    Url = new Uri(url)
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
            GameModel game)
        {
            var portfolio = _utility.GetPortfolio(game.GameType);
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

            var created = await _playerService.CreatePlayerAsync(configuration, query.BrandCode, query.PlayerCode, query.CurrencyCode).ConfigureAwait(false);

            return !created ? null : await _client.LoginAsync(configuration.ApiUrl, loginRequest).ConfigureAwait(false);
        }
    }
}