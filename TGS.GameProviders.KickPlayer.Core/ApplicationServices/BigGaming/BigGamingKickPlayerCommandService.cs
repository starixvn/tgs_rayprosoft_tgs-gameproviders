using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.ApiClients;
using TGS.GameProviders.Core;
using TGS.GameProviders.Repositories;
using TGS.GameProviders.Repositories.Domains;
using TGS.ServiceUtility;

namespace TGS.GameProviders.KickPlayer.Core.ApplicationServices.BigGaming
{
    public class BigGamingKickPlayerCommandService : ICommandService<BigGamingPlayerKickCommand>
    {
        private readonly IBigGamingApiClient _client;
        private readonly IGameRepository _gameRepository;
        private readonly ILogger<BigGamingKickPlayerCommandService> _logger;
        private readonly IPlayerRepository _playerRepository;
        private readonly IBigGamingService _service;

        public BigGamingKickPlayerCommandService(
            ILogger<BigGamingKickPlayerCommandService> logger,
            IBigGamingApiClient client,
            IBigGamingService service,
            IPlayerRepository playerRepository,
            IGameRepository gameRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
            _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
        }

        public async ValueTask ExecuteAsync(BigGamingPlayerKickCommand command, CancellationToken cancellationToken = default)
        {
            var player = await _playerRepository.GetPlayerAsync(command.PlayerCode).ConfigureAwait(false);
            if (player == null)
            {
                return;
            }
            var gameProviderId = await _service.GetGameProviderIdAsync().ConfigureAwait(false);
            var games = await _gameRepository.GetGameList(gameProviderId, GameType.Fishing);
            var brandMerchant = await _service.GetBrandMerchantAsync(player.DefaultWalletCurrencyCode).ConfigureAwait(false);

            var tasks = new List<Task>();

            foreach (var game in games)
            {
                tasks.Add(Task.Run(async () =>
                {
                    await _client.KickPlayerGameAsync(brandMerchant.ApiUrl, brandMerchant.AgentLoginIdPassword, new()
                    {
                        Random = Guid.NewGuid().ToString(),
                        Sn = brandMerchant.SecretCode,
                        LoginId = command.PlayerCode,
                        GameType = game.ExternalId
                    })
                   .ConfigureAwait(false);
                }));
            }

            tasks.Add(Task.Run(async () =>
            {
                await _client.KickPlayerAsync(brandMerchant.ApiUrl, brandMerchant.AgentLoginIdPassword, new()
                {
                    Random = Guid.NewGuid().ToString(),
                    Sn = brandMerchant.SecretCode,
                    LoginId = command.PlayerCode
                }).ConfigureAwait(false);
            }));

            await Task.WhenAll(tasks);
        }
    }
}
