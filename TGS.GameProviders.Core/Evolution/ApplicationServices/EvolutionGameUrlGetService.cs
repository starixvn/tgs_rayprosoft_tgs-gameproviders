using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.Core.Infrastructures.Security;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core.Evolution.Models;
using TGS.GameProviders.Core.Evolution.Models.Commons.EvolutionGameUrl;
using TGS.GameProviders.Core.Extensions;

namespace TGS.GameProviders.Core.Evolution.ApplicationServices
{
    public class EvolutionGameUrlGetService : IGetService<EvolutionGameUrlQuery, GameUrlResult>
    {
        private readonly IEvolutionApiClient _client;
        private readonly IGameService _gameService;
        private readonly ILogger<EvolutionGameUrlGetService> _logger;
        private readonly IEvolutionService _service;
        private readonly ITokenProvider _tokenProvider;

        public EvolutionGameUrlGetService(
            IEvolutionApiClient client,
            IGameService gameService,
            ILogger<EvolutionGameUrlGetService> logger,
            IEvolutionService service,
            ITokenProvider tokenProvider
            )
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _gameService = gameService ?? throw new ArgumentNullException(nameof(gameService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
        }

        public async ValueTask<GameUrlResult> GetAsync(EvolutionGameUrlQuery command, CancellationToken cancellationToken = default)
        {
            var encryptedToken = await _tokenProvider.EncryptAsync(command.LoginRecordId).ConfigureAwait(false);
            var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);
            var gameProviderCulture = await _service.GetGameProviderCultureCodeAsync(command.Language).ConfigureAwait(false);
            var gameProviderCurrency = await _service.GetGameProviderCurrencyCodeAsync(command.CurrencyCode).ConfigureAwait(false);
            var getGame = await _gameService.GetCacheAsync(command.GameId.Value).ConfigureAwait(false);
            var ip = command.IPAddress;

            var category = "slots";

            var table = new EvolutionGameUrlConfigGameTable
            {
                Id = getGame.GameExternalId
            };

            if (getGame.GameType == GameType.LiveDealerLobby)
            {
                category = "baccarat_sicbo";
                table = null;
            }

            var playerGroup = default(EvolutionGameUrlPlayerGroup);

            var data = new EvolutionGameUrlRequest
            {
                Uuid = GuidExtensions.NewSequentialGuid().ToString(),
                Player = new EvolutionGameUrlPlayer
                {
                    Id = command.PlayerCode,
                    Update = false,
                    FirstName = string.Empty,
                    LastName = string.Empty,
                    Nickname = command.PlayerCode,
                    Country = "CN",
                    Language = gameProviderCulture,
                    Currency = gameProviderCurrency,
                    Session = new EvolutionGameUrlPlayerSession
                    {
                        Id = encryptedToken,
                        Ip = ip
                    },
                    Group = playerGroup
                },
                Config = new EvolutionGameUrlConfig
                {
                    Game = new EvolutionGameUrlConfigGame
                    {
                        Category = category,
                        Table = table
                    },
                    Channel = new EvolutionGameUrlConfigChannel
                    {
                        Wrapped = true,
                        Mobile = command.PlatformType == PlatformType.Mobile
                    }
                }
            };

            var responseGameUrl = await _client.GetGameUrlAsync(configuration.Ua2AuthenticationUrl, data).ConfigureAwait(false);

            if (responseGameUrl == null || responseGameUrl?.Errors != null)
            {
                return null;
            }

            var path = responseGameUrl.Entry.Substring(1, responseGameUrl.Entry.Length - 1);
            var url = $"{configuration.ApiUrl}{path}";

            return new()
            {
                Url = new Uri(url)
            };
        }
    }
}
