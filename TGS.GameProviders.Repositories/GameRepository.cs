using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.GameProviders.Core.Repositories;
using TGS.GameProviders.Core.Repositories.Models.Game;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Repositories.Entities;

namespace TGS.GameProviders.Repositories
{
    internal class GameRepository : IGameRepository
    {
        private readonly TgsContext _context;

        public GameRepository(
           TgsContext context
           )
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Task<GameModel> GetAsync(int gameId)
            => _context.Games
            .AsQueryable()
            .Where(x => x.Id == gameId)
            .Select(x => new GameModel(
              x.Id,
              (GameStatus)x.Status,
              x.ExternalId,
              x.Description,
              x.EndpointPath,
              x.RuleUrl,
              x.GameProviderId,
              (GameType)x.Type,
              (GamePlatformType)x.PlatformType,
              x.SubGame,
              x.AvailableToBrand,
              x.HasTurnoverOnTieBet,
              x.Html5,
              (string.IsNullOrEmpty(x.AvailableCurrencies)) ? null : x.AvailableCurrencies.Split(",", StringSplitOptions.RemoveEmptyEntries),
              x.Code,
              x.Name,
              x.Localization,
              x.SubGameProviderCode
                ))
            .FirstOrDefaultAsync();

        public Task<GameModel> GetAsync(Guid gameProviderId, string externalGameId)
            => _context.Games
            .AsQueryable()
            .Where(x => x.GameProviderId == gameProviderId && x.ExternalId == externalGameId)
            .Select(x => new GameModel(
              x.Id,
              (GameStatus)x.Status,
              x.ExternalId,
              x.Description,
              x.EndpointPath,
              x.RuleUrl,
              x.GameProviderId,
              (GameType)x.Type,
              (GamePlatformType)x.PlatformType,
              x.SubGame,
              x.AvailableToBrand,
              x.HasTurnoverOnTieBet,
              x.Html5,
              (string.IsNullOrEmpty(x.AvailableCurrencies)) ? null : x.AvailableCurrencies.Split(",", StringSplitOptions.RemoveEmptyEntries),
              x.Code,
              x.Name,
              x.Localization,
              x.SubGameProviderCode
                ))
            .FirstOrDefaultAsync();

        public IAsyncEnumerable<GameModel> GetGameList(Guid gameProviderId)
            => _context.Games
            .AsAsyncEnumerable()
            .Where(x => x.GameProviderId == gameProviderId)
            .Select(x => new GameModel(
              x.Id,
              (GameStatus)x.Status,
              x.ExternalId,
              x.Description,
              x.EndpointPath,
              x.RuleUrl,
              x.GameProviderId,
              (GameType)x.Type,
              (GamePlatformType)x.PlatformType,
              x.SubGame,
              x.AvailableToBrand,
              x.HasTurnoverOnTieBet,
              x.Html5,
              (string.IsNullOrEmpty(x.AvailableCurrencies)) ? null : x.AvailableCurrencies.Split(",", StringSplitOptions.RemoveEmptyEntries),
              x.Code,
              x.Name,
              x.Localization,
              x.SubGameProviderCode
                ));

        public IAsyncEnumerable<GameModel> GetGameList(Guid gameProviderId, GameType gameType)
            => _context.Games
            .AsAsyncEnumerable()
            .Where(x => x.GameProviderId == gameProviderId && x.Type == (short)gameType)
            .Select(x => new GameModel(
                x.Id,
                (GameStatus)x.Status,
                x.ExternalId,
                x.Description,
                x.EndpointPath,
                x.RuleUrl,
                x.GameProviderId,
                (GameType)x.Type,
                (GamePlatformType)x.PlatformType,
                x.SubGame,
                x.AvailableToBrand,
                x.HasTurnoverOnTieBet,
                x.Html5,
                (string.IsNullOrEmpty(x.AvailableCurrencies)) ? null : x.AvailableCurrencies.Split(",", StringSplitOptions.RemoveEmptyEntries),
                x.Code,
                x.Name,
                x.Localization,
                x.SubGameProviderCode
                ));
    }
}
