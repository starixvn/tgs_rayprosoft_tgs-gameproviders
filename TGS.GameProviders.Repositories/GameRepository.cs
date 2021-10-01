using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.Repositories.Entities;

namespace TGS.GameProviders.Repositories
{
    internal sealed class GameRepository : IGameRepository
    {
        private readonly TgsContext _context;
        private readonly ILogger<GameRepository> _logger;

        public GameRepository(
           ILogger<GameRepository> logger,
           TgsContext context
           )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Task<Game> GetGameAsync(int gameId)
         => _context.Games.AsQueryable().FirstOrDefaultAsync(x => x.Id == gameId);

        public Task<Game> GetGameAsync(Guid gameProviderId, string externalGameId)
        => _context.Games.AsQueryable().FirstOrDefaultAsync(x => x.GameProviderId == gameProviderId && x.ExternalId == externalGameId);
    }
}
