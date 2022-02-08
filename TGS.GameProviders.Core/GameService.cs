using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Tgs.GameProviders.QueryServer.Client;

namespace TGS.GameProviders.Core
{
    internal class GameService : IGameService
    {
        private readonly IGameProvidersQueryServerClient _gameProvidersQueryServerClient;
        private readonly IMemoryCache _memoryCache;

        public GameService(
            IGameProvidersQueryServerClient gameProvidersQueryServerClient,
            IMemoryCache memoryCache
            )
        {
            _gameProvidersQueryServerClient = gameProvidersQueryServerClient ?? throw new ArgumentNullException(nameof(gameProvidersQueryServerClient));
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        }

        public async ValueTask<GameInfo?> GetAsync(int gameId)
        {
            var gameViewModel = await _gameProvidersQueryServerClient.GameGetAsync(gameId).ConfigureAwait(false);
            return gameViewModel != null
                ? new GameInfo(
                    gameViewModel.GameId,
                    (GameStatus)gameViewModel.Status,
                    gameViewModel.GameExternalId,
                    gameViewModel.Description,
                    gameViewModel.EndpointPath,
                    gameViewModel.RuleUrl,
                    gameViewModel.GameProviderId,
                    (GameType)gameViewModel.GameType,
                    (GamePlatformType)gameViewModel.PlatformType,
                    gameViewModel.SubGame,
                    gameViewModel.AvailableToBrand,
                    gameViewModel.HasTurnoverOnTieBet,
                    gameViewModel.Html5,
                    gameViewModel.AvailableCurrencies,
                    gameViewModel.GameCode,
                    gameViewModel.Name,
                    gameViewModel.Localization,
                    gameViewModel.SubGameProviderCode)
                : null;
        }

        public async ValueTask<GameInfo?> GetAsync(Guid gameProviderId, string externalGameId)
        {
            var gameViewModel = await _gameProvidersQueryServerClient.GameGetAsync(gameProviderId, externalGameId).ConfigureAwait(false);
            return gameViewModel != null
                ? new GameInfo(
                    gameViewModel.GameId,
                    (GameStatus)gameViewModel.Status,
                    gameViewModel.GameExternalId,
                    gameViewModel.Description,
                    gameViewModel.EndpointPath,
                    gameViewModel.RuleUrl,
                    gameViewModel.GameProviderId,
                    (GameType)gameViewModel.GameType,
                    (GamePlatformType)gameViewModel.PlatformType,
                    gameViewModel.SubGame,
                    gameViewModel.AvailableToBrand,
                    gameViewModel.HasTurnoverOnTieBet,
                    gameViewModel.Html5,
                    gameViewModel.AvailableCurrencies,
                    gameViewModel.GameCode,
                    gameViewModel.Name,
                    gameViewModel.Localization,
                    gameViewModel.SubGameProviderCode)
                : null;
        }

        public async ValueTask<GameInfo?> GetCacheAsync(int gameId)
        {
            if (!_memoryCache.TryGetValue(gameId, out GameInfo? gameInfo))
            {
                gameInfo = await GetAsync(gameId).ConfigureAwait(false);
                if (gameInfo != null)
                {
                    _memoryCache.Set(gameId, gameInfo, TimeSpan.FromHours(1));
                }
            }
            return gameInfo;
        }

        public async IAsyncEnumerable<GameInfo> GetGameListAsync(Guid gameProviderId)
        {
            var gameViewModels = await _gameProvidersQueryServerClient.GameQueryAsync(gameProviderId).ConfigureAwait(false);

            foreach (var gameViewModel in gameViewModels)
                yield return new GameInfo(
                    gameViewModel.GameId,
                    (GameStatus)gameViewModel.Status,
                    gameViewModel.GameExternalId,
                    gameViewModel.Description,
                    gameViewModel.EndpointPath,
                    gameViewModel.RuleUrl,
                    gameViewModel.GameProviderId,
                    (GameType)gameViewModel.GameType,
                    (GamePlatformType)gameViewModel.PlatformType,
                    gameViewModel.SubGame,
                    gameViewModel.AvailableToBrand,
                    gameViewModel.HasTurnoverOnTieBet,
                    gameViewModel.Html5,
                    gameViewModel.AvailableCurrencies,
                    gameViewModel.GameCode,
                    gameViewModel.Name,
                    gameViewModel.Localization,
                    gameViewModel.SubGameProviderCode);
        }

        public async IAsyncEnumerable<GameInfo> GetGameListAsync(Guid gameProviderId, GameType gameType)
        {
            var gameViewModels = await _gameProvidersQueryServerClient.GameQueryAsync(gameProviderId, (Tgs.GameProviders.QueryServer.Client.GameType)gameType).ConfigureAwait(false);

            foreach (var gameViewModel in gameViewModels)
                yield return new GameInfo(
                    gameViewModel.GameId,
                    (GameStatus)gameViewModel.Status,
                    gameViewModel.GameExternalId,
                    gameViewModel.Description,
                    gameViewModel.EndpointPath,
                    gameViewModel.RuleUrl,
                    gameViewModel.GameProviderId,
                    (GameType)gameViewModel.GameType,
                    (GamePlatformType)gameViewModel.PlatformType,
                    gameViewModel.SubGame,
                    gameViewModel.AvailableToBrand,
                    gameViewModel.HasTurnoverOnTieBet,
                    gameViewModel.Html5,
                    gameViewModel.AvailableCurrencies,
                    gameViewModel.GameCode,
                    gameViewModel.Name,
                    gameViewModel.Localization,
                    gameViewModel.SubGameProviderCode);
        }
    }
}
