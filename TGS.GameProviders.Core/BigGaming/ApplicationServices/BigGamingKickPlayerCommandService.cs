using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core.Repositories;
using TGS.GameProviders.Domains;

namespace TGS.GameProviders.Core.BigGaming.ApplicationServices
{
    public class BigGamingKickPlayerCommandService : ICommandService<BigGamingKickPlayerCommand>
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

        public async ValueTask ExecuteAsync(BigGamingKickPlayerCommand command, CancellationToken cancellationToken = default)
        {
            var gameProviderId = await _service.GetGameProviderIdAsync().ConfigureAwait(false);
            var games = await _gameRepository.GetGameList(gameProviderId, GameType.Fishing);
            var brandMerchant = await _service.GetBrandMerchantAsync(command.CurrencyCode).ConfigureAwait(false);

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
                        GameType = game.GameExternalId
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
