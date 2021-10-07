using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.Repositories.Domains;
using TGS.GameProviders.Repositories.Domains.Models.Game;
using TGS.GameProviders.Repositories.Entities;

namespace TGS.GameProviders.Repositories
{
    internal class GameRepository : IGameRepository
    {
        private readonly TgsContext _context;
        private readonly ILogger<GameRepository> _logger;
        private readonly IMapper _mapper;


        public GameRepository(
           TgsContext context,
           ILogger<GameRepository> logger,
           IMapper mapper
           )
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public Task<GetGame> GetGameAsync(int gameId)
            => _context.Games.ProjectTo<GetGame>(_mapper.ConfigurationProvider).FirstOrDefaultAsync(x => x.GameId == gameId);
        public Task<GetGame> GetGameAsync(Guid gameProviderId, string externalGameId)
            => _context.Games.ProjectTo<GetGame>(_mapper.ConfigurationProvider).FirstOrDefaultAsync(x => x.GameProviderId == gameProviderId && x.ExternalId == externalGameId);
        public Task<GetGame[]> GetGameList(Guid gameProviderId)
            => _context.Games.Where(x => x.GameProviderId == gameProviderId).ProjectTo<GetGame>(_mapper.ConfigurationProvider).ToArrayAsync();
        public Task<GetGame[]> GetGameList(Guid gameProviderId, GameType gameType)
            => _context.Games.Where(x => x.GameProviderId == gameProviderId && x.Type == (short)gameType).ProjectTo<GetGame>(_mapper.ConfigurationProvider).ToArrayAsync();
    }
}
