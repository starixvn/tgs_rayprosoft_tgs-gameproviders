using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Tgs.GameProviders.QueryServer.Client;
using Tgs.GameProviders.QueryServer.Models;

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
            var data = await _gameProvidersQueryServerClient.GameGetAsync(gameId).ConfigureAwait(false);
            return data != null
                ? new GameInfo(
                    data.GameId,
                    (GameStatus)data.Status,
                    data.GameExternalId,
                    data.Description,
                    data.EndpointPath,
                    data.RuleUrl,
                    data.GameProviderId,
                    (GameType)data.GameType,
                    (GamePlatformType)data.PlatformType,
                    data.SubGame,
                    data.AvailableToBrand,
                    data.HasTurnoverOnTieBet,
                    data.Html5,
                    data.AvailableCurrencies,
                    data.GameCode,
                    data.Name,
                    data.Localization,
                    data.SubGameProviderCode)
                : null;
        }

        public async ValueTask<GameInfo?> GetAsync(Guid gameProviderId, string externalGameId)
        {
            var data = await _gameProvidersQueryServerClient.GameGetAsync(gameProviderId, externalGameId).ConfigureAwait(false);
            return data != null
                ? new GameInfo(
                    data.GameId,
                    (GameStatus)data.Status,
                    data.GameExternalId,
                    data.Description,
                    data.EndpointPath,
                    data.RuleUrl,
                    data.GameProviderId,
                    (GameType)data.GameType,
                    (GamePlatformType)data.PlatformType,
                    data.SubGame,
                    data.AvailableToBrand,
                    data.HasTurnoverOnTieBet,
                    data.Html5,
                    data.AvailableCurrencies,
                    data.GameCode,
                    data.Name,
                    data.Localization,
                    data.SubGameProviderCode)
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
            var gameViewModels = await _gameProvidersQueryServerClient.GameQueryAsync(gameProviderId, (Tgs.GameProviders.QueryServer.Models.GameType)gameType).ConfigureAwait(false);

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
